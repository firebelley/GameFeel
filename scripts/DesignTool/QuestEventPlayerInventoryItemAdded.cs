using GameFeel.Singleton;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestEventPlayerInventoryItemAdded : QuestEventNode
    {
        private LineEdit _idLineEdit;
        private SpinBox _requiredSpinBox;

        public override void _Ready()
        {
            base._Ready();
            Model.EventId = GameEventDispatcher.PLAYER_INVENTORY_ITEM_ADDED;

            _idLineEdit = GetNode<LineEdit>("VBoxContainer/HBoxContainer/LineEdit");
            _requiredSpinBox = GetNode<SpinBox>("VBoxContainer/HBoxContainer2/SpinBox");

            _idLineEdit.Connect("text_changed", this, nameof(OnIdChanged));
            _requiredSpinBox.Connect("value_changed", this, nameof(OnRequiredChanged));
        }

        protected override void UpdateControls()
        {
            base.UpdateControls();
            _idLineEdit.Text = Model.ItemId;
            _requiredSpinBox.Value = Model.Required;
        }

        private void OnIdChanged(string newText)
        {
            Model.ItemId = _idLineEdit.Text;
        }

        private void OnRequiredChanged(float value)
        {
            Model.Required = (int) value;
        }
    }
}