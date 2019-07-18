using GameFeel.Interface;
using Godot;
using GodotTools.Extension;

namespace GameFeel.GameObject
{
    public class Fireball : RigidBody2D, IDamageDealer
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

        public void RegisterHit(IDamageReceiver receiver)
        {
            if (_hits > 0) return;

            _hits++;
            receiver.DealDamage(1f);
            SpawnHitEffect();
            Delete();
        }

        private void SpawnHitEffect()
        {
            var fireballDeath = _resourcePreloader.InstanceScene<Node2D>("FireballDeath");
            GameWorld.EffectsLayer.AddChild(fireballDeath);
            fireballDeath.GlobalPosition = GlobalPosition;
        }

        private void OnLifetimeTimerTimeout()
        {
            Delete();
        }
    }
}