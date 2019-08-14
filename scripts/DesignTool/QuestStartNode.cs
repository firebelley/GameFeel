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
        }

        public override QuestNode.QuestModel GetSaveModel()
        {
            var saveModel = new QuestStartModel();
            saveModel.Id = "start id";
            saveModel.DisplayName = "start display name";
            return saveModel;
        }
    }
}