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
        public float MaxHp { get; private set; }

        public float CurrentHp { get; private set; }

        private DamageReceiverComponent _damageReceiverComponent;

        public override void _Ready()
        {
            CurrentHp = MaxHp;

            if (_damageReceiverComponentPath != null)
            {
                _damageReceiverComponent = GetNodeOrNull<DamageReceiverComponent>(_damageReceiverComponentPath);
                _damageReceiverComponent?.Connect(nameof(DamageReceiverComponent.DamageReceived), this, nameof(OnDamageReceived));
            }
        }

        public void Decrement(float amount)
        {
            CurrentHp -= amount;
            EmitSignal(nameof(HealthDecremented));
            if (CurrentHp <= 0f)
            {
                _damageReceiverComponent?.Disable();
                EmitSignal(nameof(HealthDepleted));
            }
        }

        public float GetHealthPercentage()
        {
            return CurrentHp / (MaxHp > 0 ? MaxHp : 1f);
        }

        private void OnDamageReceived(float damage)
        {
            Decrement(damage);
        }
    }
}