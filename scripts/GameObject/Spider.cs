using GameFeel.Interface;
using Godot;

namespace GameFeel.GameObject
{
    public class Spider : KinematicBody2D, IDamageReceiver
    {
        [Signal]
        public delegate void DamageReceived(float damage);

        private Area2D _hitboxArea;
        private Tween _shaderTween;
        private AnimatedSprite _animatedSprite;
        private ShaderMaterial _shaderMaterial;
        private float _maxHp = 10f;
        private float _hp = 10f;

        public override void _Ready()
        {
            _hitboxArea = GetNode<Area2D>("HitboxArea2D");
            _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            _shaderTween = GetNode<Tween>("ShaderTween");

            _shaderMaterial = _animatedSprite.Material as ShaderMaterial;
            _hitboxArea.Connect("body_entered", this, nameof(OnBodyEntered));
        }

        public void DealDamage(float damage)
        {
            PlayHitShadeTween();
            Camera.Shake();
            GameWorld.CreateDamageNumber(this, damage);
            _hp -= damage;
            EmitSignal(nameof(DamageReceived), damage);
        }

        public float GetCurrentHealthPercent()
        {
            return _hp / (_maxHp > 0f ? _maxHp : 1f);
        }

        private void PlayHitShadeTween()
        {
            _shaderTween.ResetAll();
            _shaderTween.InterpolateProperty(
                _shaderMaterial,
                "shader_param/_hitShadePercent",
                1.0f,
                0f,
                .3f,
                Tween.TransitionType.Quad,
                Tween.EaseType.In
            );
            _shaderTween.Start();
        }

        private void OnBodyEntered(PhysicsBody2D body)
        {
            if (body is IDamageDealer dd)
            {
                dd.RegisterHit(this);
            }
        }

    }
}