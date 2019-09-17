using GameFeel.Singleton;
using GameFeel.UI;
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

        [Export]
        private NodePath _selectableComponentPath;

        public override void _Ready()
        {
            GetNodeOrNull<DeathEffectComponent>(_deathEffectComponentPath ?? string.Empty)?.Connect(nameof(DeathEffectComponent.Killed), this, nameof(OnKilled));
            var selectableComponent = GetNodeOrNull<SelectableComponent>(_selectableComponentPath ?? string.Empty);
            selectableComponent?.Connect(nameof(SelectableComponent.SelectEnter), this, nameof(OnSelectEnter));
            selectableComponent?.Connect(nameof(SelectableComponent.SelectLeave), this, nameof(OnSelectLeave));
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

        private void OnSelectEnter()
        {
            TooltipUI.ShowItemTooltip(Id);
        }

        private void OnSelectLeave()
        {
            TooltipUI.HideTooltip();
        }
    }
}