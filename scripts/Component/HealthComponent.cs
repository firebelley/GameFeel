using Godot;

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
    }
}