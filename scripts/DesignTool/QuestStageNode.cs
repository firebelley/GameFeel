using GameFeel.Data.Model;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestStageNode : QuestNode
    {
        private LineEdit _nameLineEdit;
        private LineEdit _idLineEdit;

        public override void _Ready()
        {
            base._Ready();
            Model = new QuestStageModel();
            Model.DisplayName = "stage";
            _nameLineEdit = GetNode<LineEdit>("VBoxContainer/HBoxContainer/LineEdit");
            _idLineEdit = GetNode<LineEdit>("VBoxContainer/HBoxContainer2/LineEdit");
            _nameLineEdit.Connect("text_changed", this, nameof(OnNameChanged));
            _idLineEdit.Connect("text_changed", this, nameof(OnIdChanged));

            UpdateControls();
        }

        public override void LoadModel(QuestModel questModel)
        {
            Model = (QuestStageModel) questModel;
            UpdateControls();
        }

        private void UpdateControls()
        {
            _nameLineEdit.Text = ((QuestStageModel) Model).DisplayName;
            _idLineEdit.Text = ((QuestStageModel) Model).Id;
        }

        private void OnNameChanged(string newText)
        {
            Model.DisplayName = _nameLineEdit.Text;
        }

        private void OnIdChanged(string newText)
        {
            Model.Id = _idLineEdit.Text;
        }
    }
}