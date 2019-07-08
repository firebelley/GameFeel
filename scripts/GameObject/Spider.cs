using System;
using Godot;

namespace GameFeel.GameObject
{
    public class Spider : KinematicBody2D
    {
        private Area2D _hitboxArea;

        public override void _Ready()
        {
            _hitboxArea = GetNode<Area2D>("HitboxArea2D");

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
            }
        }
    }
}