using GameFeel.Singleton;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestEventEntityKilled : QuestEventNode
    {
        private QuestItemSelector _entityItemSelector;
        private SpinBox _requiredSpinBox;

        public override void _Ready()
        {
            base._Ready();
            Model.EventId = GameEventDispatcher.ENTITY_KILLED;

            _entityItemSelector = GetNode<QuestItemSelector>("VBoxContainer/HBoxContainer/QuestItemSelector");
            _requiredSpinBox = GetNode<SpinBox>("VBoxContainer/HBoxContainer2/SpinBox");

            _entityItemSelector.Connect(nameof(QuestItemSelector.ItemSelected), this, nameof(OnItemSelected));
            _requiredSpinBox.Connect("value_changed", this, nameof(OnRequiredChanged));
        }

        protected override void UpdateControls()
        {
            base.UpdateControls();
            _entityItemSelector.SetItemId(Model.ItemId);
            _requiredSpinBox.Value = Model.Required;
        }

        private void OnItemSelected(string id)
        {
            Model.ItemId = id;
        }

        private void OnRequiredChanged(float value)
        {
            Model.Required = (int) value;
        }
    }
}