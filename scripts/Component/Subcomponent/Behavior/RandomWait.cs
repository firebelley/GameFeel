using Godot;

namespace GameFeel.Component.Subcomponent.Behavior
{
    public class RandomWait : BehaviorNode
    {
        [Export]
        private float _minWait = 1f;
        [Export]
        private float _maxWait = 2f;

        private Timer _timer;

        public override void _Ready()
        {
            base._Ready();
            _timer = GetNode<Timer>("Timer");
            _timer.Connect("timeout", this, nameof(OnTimerTimeout));
        }

        protected override void InternalEnter()
        {
            _timer.WaitTime = Main.RNG.RandfRange(_minWait, _maxWait);
            _timer.Start();
        }

        protected override void InternalLeave()
        {
            _timer.Stop();
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