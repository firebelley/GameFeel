using GameFeel.GameObject;
using Godot;
using GodotTools.Extension;
using GodotTools.Logic.Interface;

namespace GameFeel.Component.State
{
    public class EnemyStateWander : EnemyState
    {
        [Export]
        private NodePath _pursueStateNodePath;
        [Export]
        private NodePath _animatedSpritePath;
        [Export]
        private float _pathfindMinTime = 1.5f;
        [Export]
        private float _pathfindMaxTime = 2.5f;
        [Export]
        private int _wanderDistance = 100;
        [Export]
        private int _detectionDistance = 75;

        private Timer _timer;
        private AnimatedSprite _animatedSprite;

        private IStateExector _pursueState;

        public override void _Ready()
        {
            base._Ready();
            _timer = GetNode<Timer>("Timer");
            _animatedSprite = GetNode<AnimatedSprite>(_animatedSpritePath);

            if (_pursueStateNodePath != null)
            {
                _pursueState = GetNode(_pursueStateNodePath) as IStateExector;
            }
        }

        public override void StateActive()
        {
            if (_timer.IsStopped())
            {
                var toPos = Vector2.Right.Rotated(Main.RNG.RandfRange(0f, Mathf.Pi * 2f));
                toPos *= _wanderDistance;
                toPos += _parent.MetaSpawnPosition == Vector2.Zero ? _parentOwner.GlobalPosition : _parent.MetaSpawnPosition;
                _parent.MetaPathfindComponent.UpdateStraightPath(_parentOwner.GlobalPosition, toPos);
                _timer.WaitTime = Main.RNG.RandfRange(_pathfindMinTime, _pathfindMaxTime);
                _timer.Start();
            }

            if (_parentOwner.GlobalPosition.DistanceSquaredTo(GetTree().GetFirstNodeInGroup<Player>(Player.GROUP)?.GlobalPosition ?? Vector2.Zero) < _detectionDistance * _detectionDistance)
            {
                _parent.StateMachine.ChangeState(_pursueState);
            }

            if (_parent.MetaPathfindComponent.Velocity.x < -5f)
            {
                _animatedSprite.FlipH = true;
            }
            else if (_parent.MetaPathfindComponent.Velocity.x > 5f)
            {
                _animatedSprite.FlipH = false;
            }

            if (_parent.MetaPathfindComponent.Velocity.LengthSquared() > 4f)
            {
                _animatedSprite.Play(EnemyAIComponent.META_ANIM_RUN);
            }
            else
            {
                _animatedSprite.Play(EnemyAIComponent.META_ANIM_IDLE);
            }
        }

        public override void StateEntered()
        {

        }

        public override void StateLeft()
        {

        }

    }
}