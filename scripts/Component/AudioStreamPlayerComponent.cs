using Godot;

namespace GameFeel.Component
{
    public class AudioStreamPlayerComponent : AudioStreamPlayer
    {
        private const float MIN_PITCH = .75f;
        private const float MAX_PITCH = 1.25f;

        [Export]
        private bool _randomPitch;

        public override void _Ready()
        {
            RandomizePitch();
            Connect("finished", this, nameof(OnFinished));
        }

        private void RandomizePitch()
        {
            if (_randomPitch)
            {
                PitchScale = Main.RNG.RandfRange(MIN_PITCH, MAX_PITCH);
            }
        }

        private void OnFinished()
        {
            RandomizePitch();
        }
    }
}