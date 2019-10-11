using System.Collections.Generic;
using System.Linq;
using GameFeel.Data.Model;
using GameFeel.Resource;
using Godot;
using GodotTools.Extension;
using GodotTools.Util;

namespace GameFeel.Singleton
{
    public class QuestTracker : Node
    {
        private const string QUEST_NODE_PATH = "res://scenes/Resource/Quest.tscn";

        [Signal]
        public delegate void PreQuestStarted(Quest quest);

        public static QuestTracker Instance { get; private set; }

        private static HashSet<string> _activeQuestIds = new HashSet<string>();
        private static HashSet<string> _completedQuestIds = new HashSet<string>();
        private static HashSet<string> _completedModelIds = new HashSet<string>();
        private static HashSet<QuestModel> _activeQuestModels = new HashSet<QuestModel>();

        private static PackedScene _questScene;

        public override void _Ready()
        {
            Instance = this;
            _questScene = GD.Load(QUEST_NODE_PATH) as PackedScene;
        }

        public static void StartQuest(string questGuid)
        {
            if (MetadataLoader.QuestIdToMetadata.ContainsKey(questGuid))
            {
                if (!IsQuestAvailable(questGuid))
                {
                    return;
                }
                var quest = _questScene.Instance() as Quest;

                quest.LoadQuest(questGuid);
                _activeQuestIds.Add(quest.QuestSaveModel.Start.Id);

                Instance.AddChild(quest);
                quest.Connect(nameof(Quest.QuestCompleted), Instance, nameof(OnQuestCompleted));
                quest.Connect(nameof(Quest.QuestModelActivated), Instance, nameof(OnQuestModelActivated));
                quest.Connect(nameof(Quest.QuestModelDeactivated), Instance, nameof(OnQuestModelDeactivated));

                Instance.EmitSignal(nameof(PreQuestStarted), quest);
                quest.Start();
            }
            else
            {
                Logger.Error("No quest with path " + questGuid + " exists");
            }
        }

        public static QuestModel GetActiveModel(string modelId)
        {
            return _activeQuestModels.Where(x => x.Id == modelId).FirstOrDefault();
        }

        public static bool IsModelIdComplete(string modelId)
        {
            return _completedModelIds.Contains(modelId);
        }

        public static Quest GetActiveQuestContainingModelId(string modelId)
        {
            return Instance.GetChildren<Quest>().FirstOrDefault(x => x.ContainsModelId(modelId));
        }

        public static bool IsQuestAvailable(string questId)
        {
            return !_activeQuestIds.Contains(questId) && !IsQuestCompleted(questId);
        }

        public static bool IsQuestCompleted(string questGuid)
        {
            return _completedQuestIds.Contains(questGuid);
        }

        private void OnQuestCompleted(Quest quest, string modelId)
        {
            var questId = quest.QuestSaveModel.Start.Id;
            _completedQuestIds.Add(questId);
        }

        private void OnQuestModelActivated(Quest quest, string modelId)
        {
            _activeQuestModels.Add(quest.QuestSaveModel.IdToModelMap[modelId]);
        }

        private void OnQuestModelDeactivated(Quest quest, string modelId)
        {
            var model = quest.QuestSaveModel.IdToModelMap[modelId];
            if (_activeQuestModels.Contains(model))
            {
                _activeQuestModels.Remove(model);
                _completedModelIds.Add(model.Id);
            }
        }
    }
}