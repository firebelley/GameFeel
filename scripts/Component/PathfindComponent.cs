using GameFeel.GameObject;
using Godot;
using GodotTools.Extension;

namespace GameFeel.Component
{
    [Tool]
    public class PathfindComponent : Node
    {
        private const float MAX_AHEAD = 20f;
        private const float TIME_DIFF_PERCENT = .25f;

        [Export]
        private float _maxSpeed;
        [Export]
        private float _acceleration;

        public Vector2 Velocity { get; private set; }

        public Curve2D Curve { get; private set; } = new Curve2D();

        private Timer _timer;

        private float _currentT;
        private bool _enabled = true;
        private float _baseWaitTime;
        private KinematicBody2D _owner;

        public override void _Ready()
        {
            _owner = GetOwner<KinematicBody2D>();
            _timer = GetNode<Timer>("Timer");

            _baseWaitTime = _timer.WaitTime;
            _timer.Connect("timeout", this, nameof(OnTimerTimeout));
        }

        public override string _GetConfigurationWarning()
        {
            if (!IsInstanceValid(GetOwner()) || !(GetOwner() is KinematicBody2D))
            {
                return "Owner must be a " + nameof(KinematicBody2D);
            }
            return string.Empty;
        }

        public void UpdateVelocity()
        {
            var destinationPoint = Curve.InterpolateBaked(_currentT);
            var acceleration = Vector2.Zero;

            if (_owner.GlobalPosition.DistanceSquaredTo(destinationPoint) < MAX_AHEAD * MAX_AHEAD)
            {
                _currentT += _maxSpeed * GetProcessDeltaTime();
            }

            if (_currentT < (Curve.GetBakedLength()))
            {
                acceleration = (destinationPoint - _owner.GlobalPosition).Normalized() * _acceleration;
            }
            else
            {
                Velocity = Vector2.Zero;
            }

            Velocity += acceleration * GetProcessDeltaTime();
            Velocity = Velocity.Clamped(_maxSpeed);
            Velocity = _owner.MoveAndSlide(Velocity);
        }

        public void Disable()
        {
            _enabled = false;
        }

        public void Enable()
        {
            _enabled = true;
        }

        public void UpdatePath()
        {
            var player = GetTree().GetFirstNodeInGroup<Player>(Player.GROUP);
            if (player != null)
            {
                Curve = GameWorld.GetPathCurve(_owner.GlobalPosition, player.GlobalPosition, 10f);
                _currentT = 0f;
            }
        }

        private void OnTimerTimeout()
        {
            // TODO: ALWAYS ENABLED
            if (_enabled)
            {
                UpdatePath();
            }

            var start = _baseWaitTime * (1 - TIME_DIFF_PERCENT);
            var end = _baseWaitTime + _baseWaitTime * TIME_DIFF_PERCENT;
            _timer.WaitTime = Main.RNG.RandfRange(start, end);
            _timer.Start();
        }
    }
}