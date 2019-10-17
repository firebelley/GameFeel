using GameFeel.GameObject;
using Godot;
using GodotApiTools.Extension;
using GodotApiTools.Logic.Interface;

namespace GameFeel.Component.State
{
    public class EnemyStatePursue : EnemyState
    {
        [Export]
        private NodePath _attackStateNodePath;
        [Export]
        private NodePath _animatedSpritePath;
        [Export]
        private int _initiateAttackRange = 50;
        [Export]
        private float _minPathfindTime = 1f;
        [Export]
        private float _maxPathfindTime = 2f;

        private AnimatedSprite _animatedSprite;
        private IStateExector _attackState;

        private Timer _pathUpdateTimer;

        public override void _Ready()
        {
            base._Ready();
            _animatedSprite = GetNode<AnimatedSprite>(_animatedSpritePath);
            _pathUpdateTimer = GetNode<Timer>("PathUpdateTimer");
            _attackState = GetNodeOrNull<IStateExector>(_attackStateNodePath ?? string.Empty) as IStateExector;
            _pathUpdateTimer.Connect("timeout", this, nameof(OnPathfindUpdateTimerTimeout));
        }

        public override void StateEntered()
        {
            _parent.MetaPathfindComponent.Connect(nameof(PathfindComponent.PathEndReached), this, nameof(OnPathEndReached));
            UpdatePathAndTimer();
        }

        public override void StateActive()
        {
            if (_parent.MetaPathfindComponent.Velocity.x < -5f)
            {
                _animatedSprite.FlipH = true;
            }
            else if (_parent.MetaPathfindComponent.Velocity.x > 5f)
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
            this.DisconnectAllSignals(_parent.MetaPathfindComponent);
            _pathUpdateTimer.Stop();
            _parent.MetaPathfindComponent.ClearPath();
        }

        private void UpdatePathAndTimer()
        {
            _pathUpdateTimer.WaitTime = Main.RNG.RandfRange(_minPathfindTime, _maxPathfindTime);
            _pathUpdateTimer.Start();
            _parent.MetaPathfindComponent.UpdatePathToPlayer();
        }

        private void OnPathfindUpdateTimerTimeout()
        {
            UpdatePathAndTimer();
        }

        private void OnPathEndReached()
        {
            UpdatePathAndTimer();
        }
    }
}