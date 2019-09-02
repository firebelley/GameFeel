using GameFeel.Component.Subcomponent.Behavior;

namespace GameFeel.Component
{
    public class BehaviorTreeComponent : Selector
    {
        private bool _shouldEnter = false;

        public override void _Ready()
        {
            base._Ready();
            CallDeferred(nameof(Enter));
        }

        public override void Enter()
        {
            SetProcess(true);
            base.Enter();
        }

        protected override void Tick()
        {

            if (_shouldEnter)
            {
                _shouldEnter = false;
                Enter();
            }
        }

        protected override void Leave(Status status)
        {
            base.Leave(status);
            SetProcess(true);
            _shouldEnter = true;
        }
    }
}