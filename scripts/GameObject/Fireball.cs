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

        public override void _Ready()
        {
            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
        }

        public void SetDirection(Vector2 direction)
        {
            LinearVelocity = direction.Normalized() * SPEED;
        }

        public void Delete()
        {
            var fireballDeath = _resourcePreloader.InstanceScene<Node2D>("FireballDeath");
            Main.EffectsLayer.AddChild(fireballDeath);
            fireballDeath.GlobalPosition = GlobalPosition;
            _animationPlayer.Play(ANIM_DELETE);
        }

        public void RegisterHit(IDamageReceiver receiver)
        {
            receiver.DealDamage(1f);
            Delete();
        }
    }
}