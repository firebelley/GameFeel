using Godot;
using GodotTools.Extension;

namespace GameFeel.Component
{
    [Tool]
    public class DamageReceiverComponent : Area2D
    {
        [Signal]
        public delegate void DamageReceived(float damage);

        [Export]
        private Shape2D _shape
        {
            get
            {
                return _realShape;
            }

            set
            {
                _realShape = value;
                if (_realShape != null)
                {
                    _realShape.ResourceLocalToScene = true;
                }
                if (IsInstanceValid(_collisionShape2d))
                {
                    _collisionShape2d.Shape = _realShape;
                }
            }
        }

        [Export]
        private bool _showDamageNumber = true;
        [Export]
        private float _cameraShakeMagnitude = 1f;
        [Export]
        private bool _flashScreen = false;

        private Shape2D _realShape;
        private CollisionShape2D _collisionShape2d;

        public override void _Ready()
        {
            _collisionShape2d = GetNode<CollisionShape2D>("CollisionShape2D");
            Connect("body_entered", this, nameof(OnBodyEntered));
        }

        public void HandleHit(DamageDealerComponent damageDealer)
        {
            Camera.Shake(_cameraShakeMagnitude);
            if (_flashScreen)
            {
                Camera.Flash();
            }
            if (_showDamageNumber)
            {
                GameZone.CreateDamageNumber(this, damageDealer.Damage);
            }
            EmitSignal(nameof(DamageReceived), damageDealer.Damage);
        }

        private void OnBodyEntered(PhysicsBody2D body)
        {
            var otherDamageComponent = body.GetFirstNodeOfType<DamageDealerComponent>();
            otherDamageComponent?.HandleHit(this);
        }
    }
}