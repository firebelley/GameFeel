using Godot;

namespace GameFeel.Component
{
    [Tool]
    public class ProjectileDeleterComponent : Node
    {
        [Signal]
        public delegate void Deleted();

        [Export]
        private NodePath _particlesPath;
        [Export]
        private NodePath _hideOnDeletePath;

        private Particles2D _particles;
        private Node2D _hideOnDelete;
        private Timer _deleteTimer;
        private RigidBody2D _owner;

        private float _maxTravelDistance;
        private Vector2 _startPosition;
        private bool _checkDistance;
        private bool _isDeleting;

        public override void _Ready()
        {
            if (_particlesPath != null)
            {
                _particles = GetNode(_particlesPath) as Particles2D;
            }
            if (_hideOnDeletePath != null)
            {
                _hideOnDelete = GetNode(_hideOnDeletePath) as Node2D;
            }

            _owner = Owner as RigidBody2D;

            _deleteTimer = GetNode<Timer>("DeleteTimer");
            _deleteTimer.Connect("timeout", this, nameof(OnDeleteTimerTimeout));
        }

        public override string _GetConfigurationWarning()
        {
            if (!IsInstanceValid(_owner))
            {
                return "Owner must be " + nameof(RigidBody2D);
            }
            return string.Empty;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (IsDistanceCheckEnabled())
            {
                Delete();
            }
        }

        public void Delete()
        {
            if (_isDeleting) return;
            _isDeleting = true;

            if (IsInstanceValid(_owner))
            {
                _owner.LinearVelocity = Vector2.Zero;
                _owner.CollisionLayer = 0;
                _owner.CollisionMask = 0;
                _owner.Sleeping = true;
            }

            if (IsInstanceValid(_particles))
            {
                _particles.Emitting = false;
            }

            if (IsInstanceValid(_hideOnDelete))
            {
                _hideOnDelete.Visible = false;
            }

            _deleteTimer.Start();

            EmitSignal(nameof(Deleted));
        }

        public void SetTravelDistance(float distance)
        {
            if (IsInstanceValid(_owner))
            {
                _checkDistance = true;
                _startPosition = _owner.GlobalPosition;
                _maxTravelDistance = distance;
            }
        }

        private bool IsDistanceCheckEnabled()
        {
            return _checkDistance &&
                IsInstanceValid(_owner) &&
                _owner.GlobalPosition.DistanceSquaredTo(_startPosition) > _maxTravelDistance * _maxTravelDistance;
        }

        private void OnDeleteTimerTimeout()
        {
            if (IsInstanceValid(Owner))
            {
                Owner.QueueFree();
            }
        }
    }
}