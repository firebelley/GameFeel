using GameFeel.Effect;
using Godot;

namespace GameFeel.GameObject
{
    public class Spider : KinematicBody2D
    {
        private Area2D _hitboxArea;
        private Tween _shaderTween;
        private AnimatedSprite _animatedSprite;
        private ShaderMaterial _shaderMaterial;

        public override void _Ready()
        {
            _hitboxArea = GetNode<Area2D>("HitboxArea2D");
            _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            _shaderTween = GetNode<Tween>("ShaderTween");

            _shaderMaterial = _animatedSprite.Material as ShaderMaterial;
            _hitboxArea.Connect("body_entered", this, nameof(OnBodyEntered));
        }

        private void OnBodyEntered(PhysicsBody2D body)
        {
            if (body is Fireball fb)
            {
                var scene = GD.Load("res://scenes/Effect/FireballDeath.tscn") as PackedScene;
                var node = scene.Instance() as Node2D;
                GetParent().AddChild(node);
                node.GlobalPosition = _hitboxArea.GlobalPosition;
                fb.Delete();
                PlayTween();
                Camera.Shake();

                var damageScene = GD.Load("res://scenes/Effect/DamageNumber.tscn") as PackedScene;
                var damageNumber = damageScene.Instance() as DamageNumber;
                GetParent().AddChild(damageNumber);
                damageNumber.SetNumber(10);
                damageNumber.GlobalPosition = GlobalPosition;
            }
        }

        private void PlayTween()
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
    }
}