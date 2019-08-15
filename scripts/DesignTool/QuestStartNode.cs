using Godot;

namespace GameFeel.DesignTool
{
    public class QuestStartNode : QuestNode
    {
        public class QuestStartModel : QuestNode.QuestModel
        {

        }

        public override void _Ready()
        {
            base._Ready();
            Model = new QuestStartModel();
            Model.DisplayName = "start";
        }

        public override void LoadModel(QuestModel questModel)
        {
            Model = (QuestStartModel) questModel;
        }
    }
}