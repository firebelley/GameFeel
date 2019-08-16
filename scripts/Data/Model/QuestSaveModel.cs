using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GameFeel.Data.Model
{
    public class QuestSaveModel
    {
        public QuestStartModel Start = new QuestStartModel();

        public List<QuestStageModel> Stages = new List<QuestStageModel>();
        public List<QuestEventModel> Events = new List<QuestEventModel>();
        public Dictionary<string, List<string>> RightConnections = new Dictionary<string, List<string>>();

        [JsonIgnore]
        public Dictionary<string, QuestModel> IdToModelMap
        {
            get
            {
                var dict = new Dictionary<string, QuestModel>();
                Stages.ForEach(x => dict.Add(x.Id, x));
                Events.ForEach(x => dict.Add(x.Id, x));
                return dict;
            }
        }

        public void AddRightConnection(string fromId, string toId)
        {
            if (!RightConnections.ContainsKey(fromId))
            {
                RightConnections.Add(fromId, new List<string>());
            }
            RightConnections[fromId].Add(toId);
        }

        public void AddEvent(QuestEventModel evt)
        {
            if (!Events.Any(x => x.Id == evt.Id))
            {
                Events.Add(evt);
            }
        }

        public void AddStage(QuestStageModel stage)
        {
            if (!Stages.Any(x => x.Id == stage.Id))
            {
                Stages.Add(stage);
            }
        }
    }
}