using GameFeel.Singleton;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestEventItemTurnedIn : QuestEventNode
    {
        private QuestItemSelector _itemSelector;
        private SpinBox _amountSpinBox;

        public override void _Ready()
        {
            base._Ready();
            Model.EventId = GameEventDispatcher.ITEM_TURNED_IN;
            _itemSelector = GetNode<QuestItemSelector>("VBoxContainer/HBoxContainer/QuestItemSelector");
            _amountSpinBox = GetNode<SpinBox>("VBoxContainer/HBoxContainer2/SpinBox");

            _amountSpinBox.Connect("value_changed", this, nameof(OnAmountChanged));
            _itemSelector.Connect(nameof(QuestItemSelector.ItemSelected), this, nameof(OnItemSelected));
        }

        protected override void UpdateControls()
        {
            base.UpdateControls();
            _itemSelector.SetItemId(Model.ItemId);
            _amountSpinBox.Value = Model.Required;
        }

        private void OnAmountChanged(float newValue)
        {
            Model.Required = (int) newValue;
        }

        private void OnItemSelected(string itemId)
        {
            Model.ItemId = itemId;
        }
    }
}