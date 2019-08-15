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
            Model = new QuestStageModel();
            Model.DisplayName = "stage";
        }

        public override void LoadModel(QuestModel questModel)
        {
            Model = (QuestStageModel) questModel;
        }
    }
}