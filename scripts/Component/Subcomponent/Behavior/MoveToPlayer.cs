using System;
using GameFeel.GameObject;
using Godot;
using GodotTools.Extension;

namespace GameFeel.Component.Subcomponent.Behavior
{
    public class MoveToPlayer : BehaviorNode
    {
        [Export]
        private float _targetDistanceFromPlayer = 10;

        public override void _Ready()
        {
            base._Ready();
        }

        public override void Enter()
        {
            if (IsNearPlayer())
            {
                Leave(Status.SUCCESS);
            }
            else
            {
                SetProcess(true);
            }
        }

        protected override void Tick()
        {
            // if (_parent.MetaPathfindComponent.Velocity.x < -5f)
            // {
            //     _animatedSprite.FlipH = true;
            // }
            // else if (_parent.MetaPathfindComponent.Velocity.x > 5f)
            // {
            //     _animatedSprite.FlipH = false;
            // }

            var owner = GetParent().GetOwnerOrNull<Node2D>();
            if (IsNearPlayer())
            {
                GD.Print("it's something");
                Leave(Status.SUCCESS);
            }
            GetOwner<KinematicBody2D>().MoveAndSlide(Vector2.Right * 25);
        }

        private bool IsNearPlayer()
        {
            var player = GetTree().GetFirstNodeInGroup<Player>(Player.GROUP);
            return GetOwner<KinematicBody2D>().GlobalPosition.DistanceSquaredTo(player.GlobalPosition) < _targetDistanceFromPlayer * _targetDistanceFromPlayer;
        }
    }
}