using System.Collections.Generic;
using System.Linq;
using Godot;

namespace GameFeel.GameObject.Environment
{
    [Tool]
    public class EnvironmentObject : StaticBody2D
    {
        private const string ANIM_DEFAULT = "default";

        private HashSet<Node2D> _trackedNodes = new HashSet<Node2D>();

        private int _numBodies;
        private AnimationPlayer _animationPlayer;
        private bool _showTransparency = false;

        public override void _Ready()
        {
            var sprite = GetNode<Sprite>("Sprite");
            var area = GetNode<Area2D>("Area2D");
            var collisionShape = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");

            var shape = collisionShape.Shape as RectangleShape2D;
            shape.Extents = sprite.RegionRect.Size / 2f;
            area.Position = sprite.Position;

            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

            area.Connect("body_entered", this, nameof(OnBodyEntered));
            area.Connect("body_exited", this, nameof(OnBodyExited));

            SetProcess(false);
        }

        public override void _Process(float delta)
        {
            var existing = _showTransparency;
            var shouldBeTransparent = _trackedNodes.Any(x => x.GlobalPosition.y <= GlobalPosition.y);
            if (shouldBeTransparent != existing)
            {
                UpdateTransparency(shouldBeTransparent);
            }
            if (_trackedNodes.Count == 0)
            {
                SetProcess(false);
            }
        }

        private void UpdateTransparency(bool transparent)
        {
            _showTransparency = transparent;
            if (_showTransparency)
            {
                _animationPlayer.Play(ANIM_DEFAULT);
            }
            else
            {
                _animationPlayer.PlayBackwards(ANIM_DEFAULT);
            }
        }

        private void OnBodyEntered(PhysicsBody2D physicsBody)
        {
            _trackedNodes.Add(physicsBody);
            SetProcess(true);
        }

        private void OnBodyExited(PhysicsBody2D physicsBody)
        {
            _trackedNodes.Remove(physicsBody);
        }
    }
}