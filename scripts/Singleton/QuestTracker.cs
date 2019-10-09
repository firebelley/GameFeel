using System.Collections.Generic;
using System.Linq;
using GameFeel.Data.Model;
using GameFeel.Resource;
using Godot;
using GodotTools.Extension;
using GodotTools.Util;
using Newtonsoft.Json;

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

        private static Dictionary<string, QuestSaveModel> _quests = new Dictionary<string, QuestSaveModel>();
        private static HashSet<string> _activeQuests = new HashSet<string>();
        private static HashSet<string> _completedQuests = new HashSet<string>();

        private static PackedScene _questScene;

        public override void _Ready()
        {
            Instance = this;
            _questScene = GD.Load(QUEST_NODE_PATH) as PackedScene;
            LoadQuests();
        }

        public static void StartQuest(string questPath)
        {
            if (_quests.ContainsKey(questPath))
            {
                if (!IsQuestAvailable(questPath))
                {
                    return;
                }
                var quest = _questScene.Instance() as Quest;

                _activeQuests.Add(questPath);
                quest.SetModel(_quests[questPath]);

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

        private void LoadQuests()
        {
            _quests.Clear();
            var dir = new Directory();
            var err = dir.Open(QUESTS_PATH);
            if (err != Error.Ok)
            {
                Logger.Error("Could not load quests code " + (int) err);
                return;
            }

            dir.ListDirBegin();

            while (true)
            {
                var path = dir.GetNext();
                if (string.IsNullOrEmpty(path))
                {
                    break;
                }

                if (path.EndsWith(QUEST_EXTENSION))
                {
                    LoadQuest(path);
                }
            }

            dir.ListDirEnd();
        }

        private void LoadQuest(string fileName)
        {
            var fullPath = QUESTS_PATH + fileName;
            var file = new File();
            var err = file.OpenCompressed(fullPath, (int) File.ModeFlags.Read, (int) File.CompressionMode.Gzip);
            if (err != Error.Ok)
            {
                Logger.Error("Could not load quest " + fileName + " error code " + err);
                file.Close();
                return;
            }
            var json = file.GetAsText();
            file.Close();
            var saveModel = JsonConvert.DeserializeObject<QuestSaveModel>(json);
            if (saveModel != null)
            {
                if (_quests.ContainsKey(fullPath))
                {
                    Logger.Error("Quests already has file " + fullPath);
                }
                _quests[fullPath] = saveModel;
            }
            else
            {
                Logger.Error("Could not deserialize quest " + fileName);
            }
        }

        private void OnQuestCompleted(Quest quest, string modelId)
        {
            var questId = quest.GetQuestSaveModel().Start.Id;
            _completedQuests.Add(questId);
        }
    }
}