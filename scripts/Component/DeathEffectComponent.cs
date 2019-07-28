using Godot;
using GodotTools.Extension;

namespace GameFeel.Component
{
    [Tool]
    public class DeathEffectComponent : Node2D
    {
        [Export]
        private PackedScene _deathScene;

        public override void _Ready()
        {
            GetOwner()?.GetFirstNodeOfType<HealthComponent>()?.Connect(nameof(HealthComponent.HealthDepleted), this, nameof(OnHealthDepleted));
        }

        public override string _GetConfigurationWarning()
        {
            if (!IsInstanceValid(GetOwner()) || GetOwner().GetFirstNodeOfType<HealthComponent>() == null)
            {
                return "Owner must have a " + nameof(HealthComponent);
            }
            return string.Empty;
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