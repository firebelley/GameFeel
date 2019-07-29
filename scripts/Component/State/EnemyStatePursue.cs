using GameFeel.GameObject;
using Godot;
using GodotTools.Extension;
using GodotTools.Logic.Interface;

namespace GameFeel.Component.State
{
    public class EnemyStatePursue : EnemyState
    {
        [Export]
        private NodePath _attackStateNodePath;
        [Export]
        private NodePath _animatedSpritePath;
        [Export]
        private NodePath _pathfindComponentPath;
        [Export]
        private int _initiateAttackRange = 50;

        private PathfindComponent _pathfindComponent;
        private AnimatedSprite _animatedSprite;
        private IStateExector _attackState;

        public override void _Ready()
        {
            base._Ready();
            _pathfindComponent = GetNode<PathfindComponent>(_pathfindComponentPath);
            _animatedSprite = GetNode<AnimatedSprite>(_animatedSpritePath);

            if (_attackStateNodePath != null)
            {
                _attackState = GetNode(_attackStateNodePath) as IStateExector;
            }
        }

        public override void StateEntered()
        {
            _pathfindComponent.EnablePathTimer();
            _pathfindComponent.UpdatePath();
            _animatedSprite.Play(EnemyAIComponent.META_ANIM_RUN);
        }

        public override void StateActive()
        {
            _pathfindComponent.UpdateVelocity();

            if (_pathfindComponent.Velocity.x < -5f)
            {
                _animatedSprite.FlipH = true;
            }
            else if (_pathfindComponent.Velocity.x > 5f)
            {
                _animatedSprite.FlipH = false;
            }

            var owner = GetParent().GetOwnerOrNull<Node2D>();
            var player = owner.GetTree().GetFirstNodeInGroup<Player>(Player.GROUP);
            if (player != null)
            {
                if (player.GlobalPosition.DistanceSquaredTo(owner.GlobalPosition) < _initiateAttackRange * _initiateAttackRange)
                {
                    _parent.StateMachine.ChangeState(_attackState);
                }
            }
        }

        public override void StateLeft()
        {
            _pathfindComponent.DisablePathTimer();
        }
    }
}