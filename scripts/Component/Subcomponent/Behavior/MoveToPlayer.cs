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

        }

        private bool IsNearPlayer()
        {
            var player = GetTree().GetFirstNodeInGroup<Player>(Player.GROUP);
            return GetOwner<KinematicBody2D>().GlobalPosition.DistanceSquaredTo(player.GlobalPosition) < _targetDistanceFromPlayer * _targetDistanceFromPlayer;
        }
    }
}