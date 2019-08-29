using GameFeel.Singleton;
using Godot;

namespace GameFeel.Component
{
    [Tool]
    public class EntityDataComponent : Node
    {
        [Export]
        public string Id { get; private set; }

        [Export]
        public string DisplayName { get; private set; }

        [Export]
        private NodePath _deathEffectComponentPath;

        public override void _Ready()
        {
            GetNodeOrNull<DeathEffectComponent>(_deathEffectComponentPath ?? string.Empty)?.Connect(nameof(DeathEffectComponent.Killed), this, nameof(OnKilled));
        }

        public override string _GetConfigurationWarning()
        {
            if (_deathEffectComponentPath == null)
            {
                return "Will not emit entity killed signal without a connected " + nameof(DeathEffectComponent);
            }
            return string.Empty;
        }

        private void OnKilled()
        {
            GameEventDispatcher.DispatchEntityKilledEvent(Id);
        }
    }
}