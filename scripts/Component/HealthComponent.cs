using Godot;

namespace GameFeel.Component
{
    [Tool]
    public class HealthComponent : Node
    {
        [Signal]
        public delegate void HealthDepleted();
        [Signal]
        public delegate void HealthDecremented();

        [Export]
        private NodePath _damageReceiverComponentPath;
        [Export]
        private float _maxHp;

        private DamageReceiverComponent _damageReceiverComponent;

        private float _hp;

        public override void _Ready()
        {
            _hp = _maxHp;

            if (_damageReceiverComponentPath != null)
            {
                _damageReceiverComponent = GetNodeOrNull<DamageReceiverComponent>(_damageReceiverComponentPath);
                _damageReceiverComponent?.Connect(nameof(DamageReceiverComponent.DamageReceived), this, nameof(OnDamageReceived));
            }
        }

        public void Decrement(float amount)
        {
            _hp -= amount;
            EmitSignal(nameof(HealthDecremented));
            if (_hp <= 0f)
            {
                _damageReceiverComponent?.Disable();
                EmitSignal(nameof(HealthDepleted));
            }
        }

        public float GetHealthPercentage()
        {
            return _hp / (_maxHp > 0 ? _maxHp : 1f);
        }

        private void OnDamageReceived(float damage)
        {
            Decrement(damage);
        }
    }
}