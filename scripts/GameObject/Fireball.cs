using GameFeel.Interface;
using Godot;

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
            var fireballDeath = _resourcePreloader.GetResource("FireballDeath") as PackedScene;
            Main.CreateEffect(fireballDeath, GlobalPosition);
            _animationPlayer.Play(ANIM_DELETE);
        }

        public void RegisterHit(IDamageReceiver receiver)
        {
            receiver.DealDamage(10f);
            Delete();
        }
    }
}