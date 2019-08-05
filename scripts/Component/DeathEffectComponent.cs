using Godot;

namespace GameFeel.Component
{
    [Tool]
    public class DeathEffectComponent : Node2D
    {
        [Export]
        private PackedScene _deathScene;
        [Export]
        private NodePath _healthComponentPath;

        public override void _Ready()
        {
            GetHealthComponent()?.Connect(nameof(HealthComponent.HealthDepleted), this, nameof(OnHealthDepleted));
        }

        public override string _GetConfigurationWarning()
        {
            if (!IsInstanceValid(GetOwner()) || GetHealthComponent() == null)
            {
                return "Must have a " + nameof(HealthComponent);
            }
            return string.Empty;
        }

        private HealthComponent GetHealthComponent()
        {
            if (_healthComponentPath != null)
            {
                return GetNodeOrNull<HealthComponent>(_healthComponentPath);
            }
            return null;
        }

        private void OnHealthDepleted()
        {
            if (_deathScene == null) return;
            var death = _deathScene.Instance() as Node2D;
            GameZone.EffectsLayer.AddChild(death);
            death.GlobalPosition = GlobalPosition;
            GetOwner().QueueFree();
        }
    }
}