using GameFeel.Component.Subcomponent.Behavior;
using Godot;

namespace GameFeel.Component
{
    public class BehaviorTreeComponent : Selector
    {
        [Export]
        private int _aggroRange = 50;
        [Export]
        private NodePath _animatedSpritePath;
        [Export]
        private NodePath _pathfindComponentPath;

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
            Blackboard.Owner = GetOwnerOrNull<Node2D>();
            Blackboard.SpawnPosition = GetOwnerOrNull<Node2D>()?.GlobalPosition ?? Blackboard.SpawnPosition;
            Blackboard.AnimatedSprite = GetNodeOrNull<AnimatedSprite>(_animatedSpritePath ?? string.Empty);
            Blackboard.PathfindComponent = GetNodeOrNull<PathfindComponent>(_pathfindComponentPath ?? string.Empty);
            Blackboard.AggroRange = _aggroRange;
        }
    }
}