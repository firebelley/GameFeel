using Godot;

namespace GameFeel.DesignTool
{
    public class QuestStartNode : QuestNode
    {
        private LineEdit _nameLineEdit;
        private LineEdit _idLineEdit;

        public class QuestStartModel : QuestNode.QuestModel
        {

        }

        public override void _Ready()
        {
            base._Ready();
            Model = new QuestStartModel();
            Model.DisplayName = "start";

            _nameLineEdit = GetNode<LineEdit>("VBoxContainer/HBoxContainer/LineEdit");
            _idLineEdit = GetNode<LineEdit>("VBoxContainer/HBoxContainer2/LineEdit");
            _nameLineEdit.Connect("text_changed", this, nameof(OnNameChanged));
            _idLineEdit.Connect("text_changed", this, nameof(OnIdChanged));

            UpdateControls();
        }

        public override void LoadModel(QuestModel questModel)
        {
            Model = (QuestStartModel) questModel;
            UpdateControls();
        }

        private void UpdateControls()
        {
            _nameLineEdit.Text = ((QuestStartModel) Model).DisplayName;
            _idLineEdit.Text = ((QuestStartModel) Model).Id;
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