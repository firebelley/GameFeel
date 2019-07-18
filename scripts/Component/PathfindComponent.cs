using GameFeel.GameObject;
using Godot;
using GodotTools.Extension;

namespace GameFeel.Component
{
    [Tool]
    public class PathfindComponent : Node
    {
        private const float MAX_AHEAD = 20f;

        [Export]
        private float _maxSpeed;
        [Export]
        private float _acceleration;

        public Vector2 Velocity { get; private set; }

        private Curve2D _curve = new Curve2D();
        private Timer _timer;

        private float _currentT;
        private bool _enabled = true;
        private KinematicBody2D _owner;

        public override void _Ready()
        {
            _owner = GetOwner<KinematicBody2D>();
            _timer = GetNode<Timer>("Timer");
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
            var destinationPoint = _curve.InterpolateBaked(_currentT);
            var acceleration = Vector2.Zero;

            if (_owner.GlobalPosition.DistanceSquaredTo(destinationPoint) < MAX_AHEAD * MAX_AHEAD)
            {
                _currentT += _maxSpeed * GetProcessDeltaTime();
            }

            if (_currentT < (_curve.GetBakedLength()))
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
                _curve = GameWorld.GetPathCurve(_owner.GlobalPosition, player.GlobalPosition);
                _currentT = 0f;
            }
        }

        private void OnTimerTimeout()
        {
            if (_enabled)
            {
                UpdatePath();
            }
        }
    }
}