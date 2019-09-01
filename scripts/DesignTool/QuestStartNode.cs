using GameFeel.Data.Model;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestStartNode : QuestNode
    {
        private LineEdit _nameLineEdit;
        private LineEdit _idLineEdit;

        public new QuestStartModel Model
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
        }

        public override void LoadModel(QuestModel questModel)
        {
            base.LoadModel(questModel);
            Model = (QuestStartModel) questModel;
        }

        protected override void UpdateControls()
        {
            _nameLineEdit.Text = Model.DisplayName;
            _idLineEdit.Text = Model.Id;
        }

        private void OnNameChanged(string newText)
        {
            Model.DisplayName = _nameLineEdit.Text;
        }
    }
}