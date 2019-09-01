using GameFeel.Component.Subcomponent.Behavior;

namespace GameFeel.Component
{
    public class BehaviorTree : Selector
    {
        private bool _shouldEnter = false;

        public override void _Ready()
        {
            base._Ready();
            RequestEnter();
        }

        protected override void Enter()
        {
            SetProcess(true);
            base.Enter();
        }

        protected override void Tick()
        {
            if (_shouldEnter)
            {
                _shouldEnter = false;
                RequestEnter();
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