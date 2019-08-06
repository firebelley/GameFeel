using Godot;

namespace GameFeel.Component
{
    public class IntermittentSoundComponent : AudioStreamPlayer2D
    {
        private const float MIN_PITCH = .75f;
        private const float MAX_PITCH = 1.25f;

        [Export]
        private float _minTime = 5f;
        [Export]
        private float _maxTime = 10f;

        private Timer _timer;

        public override void _Ready()
        {
            _timer = GetNode<Timer>("Timer");
            _timer.Connect("timeout", this, nameof(OnTimerTimeout));
            RestartTimer();
        }

        private void RestartTimer()
        {
            _timer.WaitTime = Main.RNG.RandfRange(_minTime, _maxTime);
            _timer.Start();
        }

        private void OnTimerTimeout()
        {
            PitchScale = Main.RNG.RandfRange(MIN_PITCH, MAX_PITCH);
            if (!Playing)
            {
                Play();
            }
            RestartTimer();
        }
    }
}