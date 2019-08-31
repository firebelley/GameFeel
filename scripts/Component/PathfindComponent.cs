using GameFeel.GameObject;
using Godot;
using GodotTools.Extension;

namespace GameFeel.Component
{
    [Tool]
    public class PathfindComponent : Node
    {
        private const float MAX_AHEAD = 10f;
        private const float MAX_AHEAD_DELTA = MAX_AHEAD * 100f;
        private const float TIME_DIFF_PERCENT = .25f;

        [Signal]
        public delegate void PathEndReached();

        [Export]
        private float _maxSpeed = 100f;
        [Export]
        private float _acceleration = 350f;
        [Export]
        private float _deceleration = 10f;

        public Vector2 Velocity { get; private set; }

        public Curve2D Curve { get; private set; } = new Curve2D();

        private float _currentT;
        private KinematicBody2D _owner;
        private bool _pathEndReached = false;

        public override void _Ready()
        {
            _owner = GetOwner<KinematicBody2D>();
        }

        public override void _Process(float delta)
        {
            UpdateVelocity();
        }

        public override string _GetConfigurationWarning()
        {
            if (!IsInstanceValid(GetOwner()) || !(GetOwner() is KinematicBody2D))
            {
                return "Owner must be a " + nameof(KinematicBody2D);
            }
            return string.Empty;
        }

        public void UpdatePathToPlayer()
        {
            _pathEndReached = false;
            var player = GetTree().GetFirstNodeInGroup<Player>(Player.GROUP);
            if (player != null)
            {
                Curve = GameZone.GetPathCurve(_owner.GlobalPosition, player.GlobalPosition, 10f);
                _currentT = 0f;
            }
        }

        public void UpdateStraightPath(Vector2 fromPos, Vector2 toPos)
        {
            _pathEndReached = false;
            Curve = GameZone.GetPathCurve(fromPos, toPos, 0f);
            _currentT = 0f;
        }

        public void ClearPath()
        {
            Curve.ClearPoints();
        }

        private void UpdateVelocity()
        {
            var acceleration = Vector2.Zero;
            if (Curve.GetPointCount() == 0)
            {
                Decelerate();
            }
            else
            {
                var destinationPoint = Curve.InterpolateBaked(_currentT);
                if (_owner.GlobalPosition.DistanceSquaredTo(destinationPoint) < MAX_AHEAD * MAX_AHEAD)
                {
                    _currentT += MAX_AHEAD_DELTA * GetProcessDeltaTime();
                }

                if (_currentT < (Curve.GetBakedLength()))
                {
                    acceleration = (destinationPoint - _owner.GlobalPosition).Normalized() * _acceleration;
                }
                else
                {
                    if (!_pathEndReached)
                    {
                        EmitSignal(nameof(PathEndReached));
                    }
                    _pathEndReached = true;
                    Decelerate();
                }
            }

            Velocity += acceleration * GetProcessDeltaTime();
            Velocity = Velocity.Clamped(_maxSpeed);
            Velocity = _owner.MoveAndSlide(Velocity);
        }

        private void Decelerate()
        {
            Velocity = Velocity.LinearInterpolate(Vector2.Zero, _deceleration * GetProcessDeltaTime());
        }
    }
}