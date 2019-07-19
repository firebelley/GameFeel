using Godot;

namespace GameFeel.GameObject
{
    public class Fireball : RigidBody2D
    {
        private const string ANIM_DELETE = "delete";
        private const float SPEED = 500f;

        private AnimationPlayer _animationPlayer;
        private ResourcePreloader _resourcePreloader;
        private Timer _lifetimeTimer;
        private int _hits;

        public override void _Ready()
        {
            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _lifetimeTimer = GetNode<Timer>("LifetimeTimer");
            _lifetimeTimer.Connect("timeout", this, nameof(OnLifetimeTimerTimeout));
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
        }

        public void SetDirection(Vector2 direction)
        {
            LinearVelocity = direction.Normalized() * SPEED;
        }

        public void Delete()
        {
            _animationPlayer.Play(ANIM_DELETE);
        }

        private void OnLifetimeTimerTimeout()
        {
            Delete();
        }
    }
}