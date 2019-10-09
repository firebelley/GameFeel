using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameFeel.Data.Model
{
    public class QuestSaveModel
    {
        public QuestStartModel Start = new QuestStartModel();

        public HashSet<QuestStageModel> Stages = new HashSet<QuestStageModel>();
        public HashSet<QuestEventModel> Events = new HashSet<QuestEventModel>();
        public HashSet<QuestCompleteModel> Completions = new HashSet<QuestCompleteModel>();
        public HashSet<QuestRewardModel> Rewards = new HashSet<QuestRewardModel>();
        public Dictionary<string, List<string>> RightConnections = new Dictionary<string, List<string>>();
        public Dictionary<string, Dictionary<string, Tuple<int, int>>> RightConnectionPorts = new Dictionary<string, Dictionary<string, Tuple<int, int>>>();

        [JsonIgnore]
        public Dictionary<string, QuestModel> IdToModelMap
        {
            get
            {
                if (_idToModelMap != null)
                {
                    return _idToModelMap;
                }

                var dict = new Dictionary<string, QuestModel>();
                List<QuestModel> questModels = new List<QuestModel>();
                questModels.AddRange(Stages);
                questModels.AddRange(Events);
                questModels.AddRange(Completions);
                questModels.AddRange(Rewards);
                questModels.Add(Start);
                questModels.ForEach(x => dict.Add(x.Id, x));
                _idToModelMap = dict;
                return _idToModelMap;
            }
        }

        [JsonIgnore]
        private Dictionary<string, QuestModel> _idToModelMap;

        public void AddRightConnection(string fromId, string toId, int fromPort, int toPort)
        {
            if (!RightConnections.ContainsKey(fromId))
            {
                RightConnections.Add(fromId, new List<string>());
                RightConnectionPorts.Add(fromId, new Dictionary<string, Tuple<int, int>>());
            }
            RightConnections[fromId].Add(toId);
            RightConnectionPorts[fromId].Add(toId, Tuple.Create(fromPort, toPort));
        }
    }
}