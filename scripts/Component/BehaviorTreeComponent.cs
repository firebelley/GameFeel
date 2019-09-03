using GameFeel.Component.Subcomponent.Behavior;
using Godot;

namespace GameFeel.Component
{
    public class BehaviorTreeComponent : Selector
    {
        private bool _shouldEnter = false;

        public Blackboard Blackboard { get; private set; } = new Blackboard();

        public override void _Ready()
        {

            base._Ready();
            CallDeferred(nameof(InitBlackboard));
            CallDeferred(nameof(Enter));
        }

        protected override void InternalEnter()
        {
            SetProcess(true);
            base.InternalEnter();
        }

        protected override void Tick()
        {
            if (_shouldEnter)
            {
                _shouldEnter = false;
                Enter();
            }
        }

        protected override void PostLeave()
        {
            SetProcess(true);
            _shouldEnter = true;
        }

        private void InitBlackboard()
        {
            Blackboard.SpawnPosition = GetOwnerOrNull<Node2D>()?.GlobalPosition ?? Blackboard.SpawnPosition;
        }
    }
}