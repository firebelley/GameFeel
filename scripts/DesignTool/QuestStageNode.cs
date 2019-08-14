using System.Collections.Generic;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestStageNode : QuestNode
    {
        public class QuestStageModel : QuestNode.QuestModel
        {

        }

        public override void _Ready()
        {
            base._Ready();
        }

        public override QuestNode.QuestModel GetSaveModel()
        {
            var saveModel = new QuestStageModel();
            saveModel.Id = "stage id";
            saveModel.DisplayName = "stage display name";
            return saveModel;
        }
    }
}