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
        }

        public override QuestNode.QuestModel GetSaveModel()
        {
            var saveModel = new QuestEventModel();
            saveModel.Id = "event id";
            saveModel.DisplayName = "event display name";
            return saveModel;
        }
    }
}