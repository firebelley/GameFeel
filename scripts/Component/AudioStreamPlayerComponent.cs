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
            if (_randomPitch)
            {
                PitchScale = Main.RNG.RandfRange(MIN_PITCH, MAX_PITCH);
            }
        }

        private void OnDamageReceived(float damage)
        {
            Play();
        }
    }
}