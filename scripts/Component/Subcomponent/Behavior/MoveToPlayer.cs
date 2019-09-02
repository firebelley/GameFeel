using GameFeel.GameObject;
using Godot;
using GodotTools.Extension;

namespace GameFeel.Component.Subcomponent.Behavior
{
    public class MoveToPlayer : BehaviorNode
    {
        [Export]
        private NodePath _pathfindComponentPath;
        [Export]
        private NodePath _animatedSpritePath;
        [Export]
        private float _targetDistanceFromPlayer = 10;
        [Export]
        private float _minPathfindTime = 1f;
        [Export]
        private float _maxPathfindTime = 2f;

        private AnimatedSprite _animatedSprite;
        private PathfindComponent _pathfindComponent;

        private Timer _pathUpdateTimer;

        public override void _Ready()
        {
            base._Ready();
            _animatedSprite = GetNodeOrNull<AnimatedSprite>(_animatedSpritePath ?? string.Empty);
            _pathUpdateTimer = GetNode<Timer>("PathUpdateTimer");
            _pathfindComponent = GetNodeOrNull<PathfindComponent>(_pathfindComponentPath ?? string.Empty);
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
                _animatedSprite.Play(EnemyAIComponent.META_ANIM_RUN);
                _pathfindComponent.Connect(nameof(PathfindComponent.PathEndReached), this, nameof(OnPathEndReached));
                UpdatePathAndTimer();
                SetProcess(true);
            }
        }

        protected override void Tick()
        {
            if (_pathfindComponent.Velocity.x < -5f)
            {
                _animatedSprite.FlipH = true;
            }
            else if (_pathfindComponent.Velocity.x > 5f)
            {
                _animatedSprite.FlipH = false;
            }

            if (IsNearPlayer())
            {
                Leave(Status.SUCCESS);
            }
        }

        protected override void Leave(Status status)
        {
            _pathfindComponent.ClearPath();
            this.DisconnectAllSignals(_pathfindComponent);
            _pathUpdateTimer.Stop();
            base.Leave(status);
        }

        private bool IsNearPlayer()
        {
            var player = GetTree().GetFirstNodeInGroup<Player>(Player.GROUP);
            var owner = GetOwnerOrNull<Node2D>();
            return IsInstanceValid(player) && IsInstanceValid(owner) && owner.GlobalPosition.DistanceSquaredTo(player.GlobalPosition) < _targetDistanceFromPlayer * _targetDistanceFromPlayer;
        }

        private void UpdatePathAndTimer()
        {
            _pathUpdateTimer.WaitTime = Main.RNG.RandfRange(_minPathfindTime, _maxPathfindTime);
            _pathUpdateTimer.Start();
            _pathfindComponent.UpdatePathToPlayer();
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