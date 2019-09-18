using GameFeel.Data.Model;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestCompleteNode : QuestNode
    {
        private LineEdit _idLineEdit;

        public new QuestCompleteModel Model
        {
            get
            {
                return (QuestCompleteModel) base.Model;
            }
            set
            {
                base.Model = value;
            }
        }

        public override void _Ready()
        {
            base._Ready();
            _idLineEdit = GetNode<LineEdit>("VBoxContainer/HBoxContainer3/LineEdit");
        }

        public override void LoadModel(QuestModel questModel)
        {
            base.LoadModel(questModel);
            Model = (QuestCompleteModel) questModel;
        }

        protected override void UpdateControls()
        {
            _idLineEdit.Text = Model.Id;
        }
    }
}