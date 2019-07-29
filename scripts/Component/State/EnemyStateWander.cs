using GameFeel.GameObject;
using Godot;
using GodotTools.Extension;
using GodotTools.Logic.Interface;

namespace GameFeel.Component.State
{
    public class EnemyStateWander : Node, IStateExector
    {
        private const string ANIM_RUN = "run";
        private const string ANIM_IDLE = "idle";

        [Export]
        private NodePath _pursueStateNodePath;

        private Timer _timer;
        private AnimatedSprite _animatedSprite;
        private Node2D _parentOwner;

        private EnemyAIComponent _parent;
        private PathfindComponent _pathfindComponent;
        private IStateExector _pursueState;

        public override void _Ready()
        {
            _parent = GetParent() as EnemyAIComponent;
            _parentOwner = _parent.GetOwner() as Node2D;
            _timer = GetNode<Timer>("Timer");
            _pathfindComponent = _parent?.Owner?.GetFirstNodeOfType<PathfindComponent>();
            _animatedSprite = _parent?.Owner?.GetFirstNodeOfType<AnimatedSprite>();

            if (_pursueStateNodePath != null)
            {
                _pursueState = GetNode(_pursueStateNodePath) as IStateExector;
            }
        }

        public void StateActive()
        {
            if (_timer.IsStopped())
            {
                var toPos = Vector2.Right.Rotated(Main.RNG.RandfRange(0f, Mathf.Pi * 2f));
                toPos *= 100f;
                // toPos += _spawnPosition;
                toPos += _parentOwner.GlobalPosition;
                _pathfindComponent.UpdatePath(_parentOwner.GlobalPosition, toPos);
                _timer.WaitTime = Main.RNG.RandfRange(1.5f, 2.5f);
                _timer.Start();
            }

            if (_parentOwner.GlobalPosition.DistanceSquaredTo(GetTree().GetFirstNodeInGroup<Player>(Player.GROUP)?.GlobalPosition ?? Vector2.Zero) < 5000f)
            {
                _parent.StateMachine.ChangeState(_pursueState);
            }

            _pathfindComponent.UpdateVelocity();
            if (_pathfindComponent.Velocity.x < -5f)
            {
                _animatedSprite.FlipH = true;
            }
            else if (_pathfindComponent.Velocity.x > 5f)
            {
                _animatedSprite.FlipH = false;
            }

            if (_pathfindComponent.Velocity.LengthSquared() > 4f)
            {
                _animatedSprite.Play(ANIM_RUN);
            }
            else
            {
                _animatedSprite.Play(ANIM_IDLE);
            }
        }

        public void StateEntered()
        {
            _pathfindComponent.DisablePathTimer();
        }

        public void StateLeft()
        {

        }

    }
}