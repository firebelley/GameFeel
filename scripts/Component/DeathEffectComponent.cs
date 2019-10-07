using GameFeel.GameObject.Effect;
using Godot;

namespace GameFeel.Component
{
    [Tool]
    public class DeathEffectComponent : Node2D
    {
        [Signal]
        public delegate void Killed();

        [Export]
        private PackedScene _deathScene;

        [Export]
        private NodePath _healthComponentPath;

        [Export]
        private NodePath _textureSourcePath;

        private Node _textureSource;

        public override void _Ready()
        {
            GetHealthComponent()?.Connect(nameof(HealthComponent.HealthDepleted), this, nameof(OnHealthDepleted));
            _textureSource = GetNodeOrNull(_textureSourcePath ?? string.Empty);
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

            if (death is EntityDeath e)
            {
                e.SetTextureFromNode(_textureSource);
            }

            EmitSignal(nameof(Killed));
            GetOwner().QueueFree();
        }
    }
}