using System.Collections.Generic;
using System.Linq;

namespace GameFeel.DesignTool
{
    public class QuestSaveModel
    {
        public QuestStartNode.QuestStartModel Start = new QuestStartNode.QuestStartModel();

        public List<QuestStageNode.QuestStageModel> Stages = new List<QuestStageNode.QuestStageModel>();
        public List<QuestEventNode.QuestEventModel> Events = new List<QuestEventNode.QuestEventModel>();
        public Dictionary<string, List<string>> RightConnections = new Dictionary<string, List<string>>();

        public void AddRightConnection(string fromId, string toId)
        {
            if (!RightConnections.ContainsKey(fromId))
            {
                RightConnections.Add(fromId, new List<string>());
            }
            RightConnections[fromId].Add(toId);
        }

        public void AddEvent(QuestEventNode.QuestEventModel evt)
        {
            if (!Events.Any(x => x.Id == evt.Id))
            {
                Events.Add(evt);
            }
        }

        public void AddStage(QuestStageNode.QuestStageModel stage)
        {
            if (!Stages.Any(x => x.Id == stage.Id))
            {
                Stages.Add(stage);
            }
        }
    }
}