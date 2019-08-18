using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GameFeel.Data.Model
{
    public class QuestSaveModel
    {
        public QuestStartModel Start = new QuestStartModel();

        public HashSet<QuestStageModel> Stages = new HashSet<QuestStageModel>();
        public HashSet<QuestEventModel> Events = new HashSet<QuestEventModel>();
        public HashSet<QuestCompleteModel> Completions = new HashSet<QuestCompleteModel>();
        public Dictionary<string, List<string>> RightConnections = new Dictionary<string, List<string>>();

        [JsonIgnore]
        public Dictionary<string, QuestModel> IdToModelMap
        {
            get
            {
                var dict = new Dictionary<string, QuestModel>();
                List<QuestModel> questModels = new List<QuestModel>();
                questModels.AddRange(Stages);
                questModels.AddRange(Events);
                questModels.AddRange(Completions);
                questModels.Add(Start);
                questModels.ForEach(x => dict.Add(x.Id, x));
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
    }
}