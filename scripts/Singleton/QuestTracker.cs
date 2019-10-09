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
        private const string QUESTS_PATH = "res://resources/quests/";
        private const string QUEST_EXTENSION = ".quest";
        private const string QUEST_NODE_PATH = "res://scenes/Resource/Quest.tscn";

        [Signal]
        public delegate void PreQuestStarted(Quest quest);

        public static QuestTracker Instance { get; private set; }

        private static HashSet<string> _activeQuests = new HashSet<string>();
        private static HashSet<string> _completedQuests = new HashSet<string>();

        private static PackedScene _questScene;

        public override void _Ready()
        {
            Instance = this;
            _questScene = GD.Load(QUEST_NODE_PATH) as PackedScene;
        }

        public static void StartQuest(string questPath)
        {
            if (MetadataLoader.QuestFileToMetadata.ContainsKey(questPath))
            {
                if (!IsQuestAvailable(questPath))
                {
                    return;
                }
                var quest = _questScene.Instance() as Quest;

                _activeQuests.Add(questPath);
                quest.SetModel(MetadataLoader.QuestFileToMetadata[questPath].QuestSaveModel);

                Instance.AddChild(quest);
                Instance.EmitSignal(nameof(PreQuestStarted), quest);

                quest.Connect(nameof(Quest.QuestCompleted), Instance, nameof(OnQuestCompleted));
                quest.Start();
            }
            else
            {
                Logger.Error("No quest with path " + questPath + " exists");
            }
        }

        public static QuestModel GetActiveModel(string modelId)
        {
            return Instance.GetChildren<Quest>().Select(x => x.GetActiveModel(modelId)).FirstOrDefault(x => x != null);
        }

        public static Quest GetActiveQuestContainingModelId(string modelId)
        {
            return Instance.GetChildren<Quest>().FirstOrDefault(x => x.ContainsModelId(modelId));
        }

        public static bool IsQuestAvailable(string questFile)
        {
            return !_activeQuests.Contains(questFile) && !IsQuestCompleted(questFile);
        }

        public static bool IsQuestCompleted(string questGuid)
        {
            return _completedQuests.Contains(questGuid);
        }

        private void OnQuestCompleted(Quest quest, string modelId)
        {
            var questId = quest.GetQuestSaveModel().Start.Id;
            _completedQuests.Add(questId);
        }
    }
}