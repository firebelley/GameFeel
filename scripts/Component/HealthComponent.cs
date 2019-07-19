using Godot;
using GodotTools.Extension;

namespace GameFeel.Component
{
    [Tool]
    public class HealthComponent : Node
    {
        [Signal]
        public delegate void HealthDepleted();

        [Export]
        private float _maxHp;

        private float _hp;

        public override void _Ready()
        {
            _hp = _maxHp;

            var damageReceiver = GetOwner()?.GetFirstNodeOfType<DamageReceiverComponent>();
            if (damageReceiver != null)
            {
                damageReceiver.Connect(nameof(DamageReceiverComponent.DamageReceived), this, nameof(OnDamageReceived));
            }
        }

        public void Decrement(float amount)
        {
            _hp -= amount;
            if (_hp <= 0f)
            {
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