using Godot;

namespace GameFeel.Component
{
    public class AttackIntentComponent : Node2D
    {
        private Particles2D _particles;

        public override void _Ready()
        {
            _particles = GetNode<Particles2D>("Particles2D");
        }

        public void Play()
        {
            _particles.Emitting = true;
        }

        public void Stop()
        {
            _particles.Emitting = false;
        }
    }
}