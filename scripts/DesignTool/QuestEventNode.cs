using GameFeel.Singleton;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestEventNode : QuestNode
    {
        public class QuestEventModel : QuestNode.QuestModel
        {

        }

        public override void _Ready()
        {
            base._Ready();
            Model = new QuestEventModel();
            Model.DisplayName = "event";
        }

        public override void LoadModel(QuestModel questModel)
        {
            Model = (QuestEventModel) questModel;
        }
    }
}