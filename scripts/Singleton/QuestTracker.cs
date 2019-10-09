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
        [Signal]
        public delegate void PreQuestStarted(string questPath);
        [Signal]
        public delegate void QuestStageStarted(string questPath, string modelGuid);
        [Signal]
        public delegate void QuestStarted(string questPath, string modelGuid);
        [Signal]
        public delegate void QuestEventCompleted(string questPath, string modelGuid);
        [Signal]
        public delegate void QuestEventStarted(string questPath, string modelGuid);
        [Signal]
        public delegate void QuestCompleted(string questPath, string modelGuid);
        [Signal]
        public delegate void QuestEventProgress(string questPath, string modelGuid);
        [Signal]
        public delegate void QuestModelActivated(string questPath, string modelGuid);

        public static QuestTracker Instance { get; private set; }

        private static HashSet<string> _activeQuestPaths = new HashSet<string>();
        private static HashSet<string> _completedQuestPaths = new HashSet<string>();
        private static HashSet<string> _activeQuestModelIds = new HashSet<string>();
        private static HashSet<string> _completedQuestModelIds = new HashSet<string>();
        private static LinkedList<Quest> _activeQuests = new LinkedList<Quest>();

        public override void _Ready()
        {
            Instance = this;

            GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventPlayerInventoryItemUpdated), this, nameof(CheckInventoryItemUpdatedCompletion));
            GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventEntityKilled), this, nameof(CheckEntityKilledCompletion));
            GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventItemTurnedIn), this, nameof(CheckTurnInCompletion));
            GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventEntityEngaged), this, nameof(CheckEntityEngagedCompletion));
            Connect(nameof(QuestEventStarted), this, nameof(OnQuestEventStarted));
        }

        public static void StartQuest(string questPath)
        {
            if (MetadataLoader.QuestFileToMetadata.ContainsKey(questPath))
            {
                if (!IsQuestAvailable(questPath))
                {
                    return;
                }

                _activeQuestPaths.Add(questPath);
                var quest = new Quest(questPath);
                _activeQuests.AddLast(quest);

                Instance.EmitSignal(nameof(PreQuestStarted), questPath);
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
            return !_activeQuestPaths.Contains(questFile) && !IsQuestCompleted(questFile);
        }

        public static bool IsQuestCompleted(string questGuid)
        {
            return _completedQuestPaths.Contains(questGuid);
        }

        private void OnQuestCompleted(Quest quest, string modelId)
        {
            var questId = quest.GetQuestSaveModel().Start.Id;
            _completedQuestPaths.Add(questId);
        }

        private void InitializeEvent(QuestEventModel eventModel)
        {
            switch (eventModel.EventId)
            {
                case GameEventDispatcher.PLAYER_INVENTORY_ITEM_UPDATED:
                    CheckInventoryItemUpdatedCompletion(eventModel.EventId, eventModel.ItemId);
                    break;
            }
        }

        private void CheckInventoryItemUpdatedCompletion(string eventGuid, string itemId)
        {
            foreach (var quest in _activeQuests)
            {
                foreach (var evt in quest.GetActiveQuestEventModels(eventGuid).Where(x => x.ItemId == itemId))
                {
                    quest.SetEventProgress(evt, PlayerInventory.GetItemCount(itemId));
                    Instance.EmitSignal(nameof(QuestEventProgress), quest.Metadata.ResourcePath, evt.Id);
                    if (quest.GetEventProgress(evt.Id) == evt.Required)
                    {
                        quest.AdvanceFromModel(evt);
                    }
                }
            }
        }

        private void CheckEntityKilledCompletion(string eventGuid, string entityId)
        {
            foreach (var quest in _activeQuests)
            {
                foreach (var evt in quest.GetActiveQuestEventModels(eventGuid).Where(x => x.ItemId == entityId))
                {
                    quest.IncrementEventProgress(evt);
                    Instance.EmitSignal(nameof(QuestEventProgress), quest.Metadata.ResourcePath, evt.Id);
                    if (quest.GetEventProgress(evt.Id) == evt.Required)
                    {
                        quest.AdvanceFromModel(evt);
                    }
                }
            }
        }

        private void CheckTurnInCompletion(string eventGuid, string modelId, string itemId, int amount)
        {
            foreach (var quest in _activeQuests)
            {
                foreach (var evt in quest.GetActiveQuestEventModels(eventGuid).Where(x => x.Id == modelId))
                {
                    quest.IncrementEventProgress(evt, amount);
                    if (quest.GetEventProgress(evt.Id) == evt.Required)
                    {
                        quest.AdvanceFromModel(evt);
                    }
                }
            }
        }

        private void CheckEntityEngagedCompletion(string eventGuid, string entityId)
        {
            foreach (var quest in _activeQuests)
            {
                foreach (var evt in quest.GetActiveQuestEventModels(eventGuid).Where(x => x.ItemId == entityId))
                {
                    quest.AdvanceFromModel(evt);
                }
            }
        }

        private void OnQuestEventStarted(string questPath, string modelId)
        {
            InitializeEvent(MetadataLoader.QuestFileToMetadata[questPath].QuestSaveModel.IdToModelMap[modelId] as QuestEventModel);
        }
    }
}