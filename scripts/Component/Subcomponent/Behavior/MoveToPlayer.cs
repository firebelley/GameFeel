using GameFeel.GameObject;
using Godot;
using GodotTools.Extension;

namespace GameFeel.Component.Subcomponent.Behavior
{
    public class MoveToPlayer : BehaviorNode
    {
        [Export]
        private float _targetDistanceFromPlayer = 10;
        [Export]
        private float _minPathfindTime = 1f;
        [Export]
        private float _maxPathfindTime = 2f;

        private Timer _pathUpdateTimer;

        public override void _Ready()
        {
            base._Ready();
            _pathUpdateTimer = GetNode<Timer>("PathUpdateTimer");
            _pathUpdateTimer.Connect("timeout", this, nameof(OnPathfindUpdateTimerTimeout));
        }

        protected override void InternalEnter()
        {
            if (IsNearPlayer())
            {
                Leave(Status.SUCCESS);
            }
            else
            {
                _root.Blackboard.PathfindComponent.Connect(nameof(PathfindComponent.PathEndReached), this, nameof(OnPathEndReached));
                UpdatePathAndTimer();
                SetProcess(true);
            }
        }

        protected override void Tick()
        {
            if (_root.Blackboard.PathfindComponent.Velocity.x < -5f)
            {
                _root.Blackboard.AnimatedSprite.FlipH = true;
            }
            else if (_root.Blackboard.PathfindComponent.Velocity.x > 5f)
            {
                _root.Blackboard.AnimatedSprite.FlipH = false;
            }

            if (IsNearPlayer())
            {
                Leave(Status.SUCCESS);
            }
        }

        protected override void InternalLeave()
        {
            _root.Blackboard.PathfindComponent.ClearPath();
            this.DisconnectAllSignals(_root.Blackboard.PathfindComponent);
            _pathUpdateTimer.Stop();
        }

        private bool IsNearPlayer()
        {
            var player = GetTree().GetFirstNodeInGroup<Player>(Player.GROUP);
            return IsInstanceValid(player) && IsInstanceValid(_root.Blackboard.Owner) && _root.Blackboard.Owner.GlobalPosition.DistanceSquaredTo(player.GlobalPosition) < _targetDistanceFromPlayer * _targetDistanceFromPlayer;
        }

        private void UpdatePathAndTimer()
        {
            _pathUpdateTimer.WaitTime = Main.RNG.RandfRange(_minPathfindTime, _maxPathfindTime);
            _pathUpdateTimer.Start();
            _root.Blackboard.PathfindComponent.UpdatePathToPlayer();
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