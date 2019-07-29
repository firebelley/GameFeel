using GameFeel.GameObject;
using Godot;
using GodotTools.Extension;
using GodotTools.Logic.Interface;

namespace GameFeel.Component.State
{
    public class EnemyStatePursue : Node, IStateExector
    {
        [Export]
        private NodePath _attackStateNodePath;

        private const string ANIM_RUN = "run";
        private const string ANIM_IDLE = "idle";
        private const float ATTACK_RANGE = 50f;

        private PathfindComponent _pathfindComponent;
        private AnimatedSprite _animatedSprite;
        private IStateExector _attackState;
        private EnemyAIComponent _parent;

        public override void _Ready()
        {
            _parent = GetParent() as EnemyAIComponent;
            _pathfindComponent = _parent?.GetOwner()?.GetFirstNodeOfType<PathfindComponent>();
            _animatedSprite = _parent?.GetOwner()?.GetFirstNodeOfType<AnimatedSprite>();

            if (_attackStateNodePath != null)
            {
                _attackState = GetNode(_attackStateNodePath) as IStateExector;
            }
        }

        public void StateEntered()
        {
            _pathfindComponent.EnablePathTimer();
            _pathfindComponent.UpdatePath();
            _animatedSprite.Play(ANIM_RUN);
        }

        public void StateActive()
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
                if (player.GlobalPosition.DistanceSquaredTo(owner.GlobalPosition) < ATTACK_RANGE * ATTACK_RANGE)
                {
                    _parent.StateMachine.ChangeState(_attackState);
                }
            }
        }

        public void StateLeft()
        {
            _pathfindComponent.DisablePathTimer();
        }
    }
}