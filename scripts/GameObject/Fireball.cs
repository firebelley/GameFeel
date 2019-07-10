using GameFeel.Interface;
using Godot;

namespace GameFeel.GameObject
{
    public class Fireball : RigidBody2D, IDamageImparter
    {
        private const string ANIM_DELETE = "delete";
        private const float SPEED = 500f;

        private AnimationPlayer _animationPlayer;

        public override void _Ready()
        {
            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
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

        }
    }
}