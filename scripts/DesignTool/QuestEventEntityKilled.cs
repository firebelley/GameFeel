using GameFeel.Data.Model;
using GameFeel.Singleton;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestEventEntityKilled : QuestEventNode
    {
        private LineEdit _idLineEdit;
        private SpinBox _requiredSpinBox;

        public override void _Ready()
        {
            base._Ready();
            ((QuestEventModel) Model).EventId = GameEventDispatcher.ENTITY_KILLED;

            _idLineEdit = GetNode<LineEdit>("VBoxContainer/HBoxContainer/LineEdit");
            _requiredSpinBox = GetNode<SpinBox>("VBoxContainer/HBoxContainer2/SpinBox");

            _idLineEdit.Connect("text_changed", this, nameof(OnIdChanged));
            _requiredSpinBox.Connect("value_changed", this, nameof(OnRequiredChanged));

            UpdateControls();
        }

        protected override void UpdateControls()
        {
            _idLineEdit.Text = ((QuestEventModel) Model).ItemId;
            _requiredSpinBox.Value = ((QuestEventModel) Model).Required;
        }

        private void OnIdChanged(string newText)
        {
            ((QuestEventModel) Model).ItemId = _idLineEdit.Text;
        }

        private void OnRequiredChanged(float value)
        {
            ((QuestEventModel) Model).Required = (int) value;
        }
    }
}