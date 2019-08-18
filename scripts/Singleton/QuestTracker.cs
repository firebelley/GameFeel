using System.Collections.Generic;
using GameFeel.Data.Model;
using GameFeel.Resource;
using Godot;
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
        public delegate void QuestAdded(Quest quest);

        public static QuestTracker Instance { get; private set; }

        private static Dictionary<string, QuestSaveModel> _quests = new Dictionary<string, QuestSaveModel>();

        private static PackedScene _questScene;

        public override void _Ready()
        {
            Instance = this;
            _questScene = GD.Load(QUEST_NODE_PATH) as PackedScene;
            LoadQuests();
            CallDeferred(nameof(StartQuest), "388998cb-c2c9-43ae-9a5f-de1c450fef5d");
        }

        public static void StartQuest(string questGuid)
        {
            if (_quests.ContainsKey(questGuid))
            {
                var quest = _questScene.Instance() as Quest;
                Instance.AddChild(quest);
                Instance.EmitSignal(nameof(QuestAdded), quest);
                quest.Start(_quests[questGuid]);
            }
            else
            {
                Logger.Error("No quest with id " + questGuid + " exists");
            }
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
            var file = new File();
            var err = file.OpenCompressed(QUESTS_PATH + fileName, (int) File.ModeFlags.Read, (int) File.CompressionMode.Gzip);
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
                if (_quests.ContainsKey(saveModel.Start.Id))
                {
                    Logger.Error("Quests already has key " + saveModel.Start.Id);
                }
                _quests[saveModel.Start.Id] = saveModel;
            }
            else
            {
                Logger.Error("Could not deserialize quest " + fileName);
            }
        }
    }
}