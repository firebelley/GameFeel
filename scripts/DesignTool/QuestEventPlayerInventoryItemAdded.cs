using GameFeel.Singleton;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestEventPlayerInventoryItemAdded : QuestEventNode
    {
        private QuestItemSelector _itemSelector;
        private SpinBox _requiredSpinBox;

        public override void _Ready()
        {
            base._Ready();
            Model.EventId = GameEventDispatcher.PLAYER_INVENTORY_ITEM_UPDATED;

            _itemSelector = GetNode<QuestItemSelector>("VBoxContainer/HBoxContainer/QuestItemSelector");
            _requiredSpinBox = GetNode<SpinBox>("VBoxContainer/HBoxContainer2/SpinBox");
            _requiredSpinBox.Connect("value_changed", this, nameof(OnRequiredChanged));
            _itemSelector.Connect(nameof(QuestItemSelector.ItemSelected), this, nameof(OnItemSelected));
        }

        protected override void UpdateControls()
        {
            base.UpdateControls();
            _itemSelector.SetItemId(Model.ItemId);
            _requiredSpinBox.Value = Model.Required;
        }

        private void OnRequiredChanged(float value)
        {
            Model.Required = (int) value;
        }

        private void OnItemSelected(string id)
        {
            Model.ItemId = id;
        }
    }
}