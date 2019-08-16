using GameFeel.Data.Model;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestStartNode : QuestNode
    {
        private LineEdit _nameLineEdit;
        private LineEdit _idLineEdit;

        protected new QuestStartModel Model
        {
            get
            {
                return (QuestStartModel) base.Model;
            }
            set
            {
                base.Model = value;
            }
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
            _nameLineEdit.Text = Model.DisplayName;
            _idLineEdit.Text = Model.Id;
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