using Godot;

namespace GameFeel.Component
{
    [Tool]
    public class ProjectileDeleterComponent : Node
    {
        [Export]
        private NodePath _particlesPath;
        [Export]
        private NodePath _hideOnDeletePath;

        private Particles2D _particles;
        private Node2D _hideOnDelete;
        private Timer _deleteTimer;

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

            _deleteTimer = GetNode<Timer>("DeleteTimer");
            _deleteTimer.Connect("timeout", this, nameof(OnDeleteTimerTimeout));
        }

        public override string _GetConfigurationWarning()
        {
            if (!IsInstanceValid(GetOwner()) || !(GetOwner() is RigidBody2D))
            {
                return "Owner must be " + nameof(RigidBody2D);
            }
            return string.Empty;
        }

        public void Delete()
        {
            if (GetOwner() is RigidBody2D rigidBody)
            {
                rigidBody.LinearVelocity = Vector2.Zero;
                rigidBody.CollisionLayer = 0;
                rigidBody.CollisionMask = 0;
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
        }

        private void OnDeleteTimerTimeout()
        {
            if (IsInstanceValid(GetOwner()))
            {
                GetOwner().QueueFree();
            }
        }
    }
}