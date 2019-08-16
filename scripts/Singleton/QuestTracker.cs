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
        public delegate void QuestStarted(string questId);

        public static QuestTracker Instance { get; private set; }

        private static Dictionary<string, QuestSaveModel> _quests = new Dictionary<string, QuestSaveModel>();

        private static PackedScene _questScene;

        public override void _Ready()
        {
            Instance = this;
            _questScene = GD.Load(QUEST_NODE_PATH) as PackedScene;
            LoadQuests();
            StartQuest("388998cb-c2c9-43ae-9a5f-de1c450fef5d");
        }

        public static void StartQuest(string questGuid)
        {
            if (_quests.ContainsKey(questGuid))
            {
                var quest = _questScene.Instance() as Quest;
                Instance.AddChild(quest);
                quest.SetQuestModel(_quests[questGuid]);
            }
        }

        private void LoadQuests()
        {
            _quests.Clear();
            var dir = new Directory();
            dir.Open(QUESTS_PATH);
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
            var err = file.Open(QUESTS_PATH + fileName, (int) File.ModeFlags.Read);
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
                _quests.Add(saveModel.Start.Id, saveModel);
                GD.Print(_quests.Count);
            }
            else
            {
                Logger.Error("Could not deserialize quest " + fileName);
            }
        }
    }
}