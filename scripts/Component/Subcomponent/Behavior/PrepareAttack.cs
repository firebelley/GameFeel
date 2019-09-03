using Godot;

namespace GameFeel.Component.Subcomponent.Behavior
{
    public class PrepareAttack : BehaviorNode
    {
        [Export]
        private NodePath _attackIntentComponentPath;
        [Export]
        private float _waitTime;

        private AttackIntentComponent _attackIntentComponent;
        private Timer _timer;

        public override void _Ready()
        {
            base._Ready();
            _attackIntentComponent = GetNodeOrNull<AttackIntentComponent>(_attackIntentComponentPath ?? string.Empty);
            _timer = GetNode<Timer>("Timer");
            _timer.Connect("timeout", this, nameof(OnTimerTimeout));
        }

        protected override void InternalEnter()
        {
            _attackIntentComponent.Play();
            _timer.WaitTime = _waitTime;
            _timer.Start();
        }

        protected override void InternalLeave()
        {
            _timer.Stop();
            _attackIntentComponent.Stop();
        }

        protected override void Tick()
        {

        }

        private void OnTimerTimeout()
        {
            Leave(Status.SUCCESS);
        }
    }
}