using System.Collections.Generic;
using System.Linq;
using GameFeel.Data.Model;
using GameFeel.Singleton;
using Godot;
using GodotApiTools.Extension;
using GodotApiTools.Util;

namespace GameFeel.Resource
{
    public class Quest : Node
    {
        [Signal]
        public delegate void QuestStageStarted(Quest quest, string modelGuid);
        [Signal]
        public delegate void QuestStarted(Quest quest, string modelGuid);
        [Signal]
        public delegate void QuestEventCompleted(Quest quest, string modelGuid);
        [Signal]
        public delegate void QuestEventStarted(Quest quest, string modelGuid);
        [Signal]
        public delegate void QuestCompleted(Quest quest, string modelGuid);
        [Signal]
        public delegate void QuestEventProgress(Quest quest, string modelGuid);
        [Signal]
        public delegate void QuestModelActivated(Quest quest, string modelGuid);
        [Signal]
        public delegate void QuestModelDeactivated(Quest quest, string modelGuid);

        public QuestSaveModel QuestSaveModel { get; private set; }
        private MetadataLoader.QuestMetadata Metadata;
        private HashSet<QuestModel> _activeModels = new HashSet<QuestModel>();
        private Dictionary<string, int> _eventProgress = new Dictionary<string, int>();

        public static bool IsQuestEventReadyForCompletion(QuestModel questModel)
        {
            if (questModel is QuestEventModel qem)
            {
                switch (qem.EventId)
                {
                    case GameEventDispatcher.ITEM_TURNED_IN:
                        return PlayerInventory.GetItemCount(qem.ItemId) >= qem.Required;
                }
            }
            return false;
        }

        public void LoadQuest(string questGuid)
        {
            Metadata = MetadataLoader.QuestIdToMetadata[questGuid];
            QuestSaveModel = Metadata.QuestSaveModel;
        }

        public List<QuestRewardModel> GetRewards(string modelId)
        {
            if (QuestSaveModel.RightConnections.ContainsKey(modelId))
            {
                return QuestSaveModel.RightConnections[modelId]
                    .Select(x => QuestSaveModel.IdToModelMap[x] as QuestRewardModel)
                    .Where(x => x != null)
                    .ToList();
            }
            return new List<QuestRewardModel>();
        }

        public void Start()
        {
            if (QuestSaveModel == null)
            {
                Logger.Error("No quest save model set before starting!");
                return;
            }
            Activate(QuestSaveModel.Start);
        }

        public bool ContainsModelId(string modelId)
        {
            return QuestSaveModel.IdToModelMap.ContainsKey(modelId);
        }

        public QuestModel GetQuestModel(string modelId)
        {
            return QuestSaveModel.IdToModelMap[modelId];
        }

        public int GetEventProgress(string modelId)
        {
            if (_eventProgress.ContainsKey(modelId))
            {
                return _eventProgress[modelId];
            }
            return 0;
        }

        public QuestModel GetActiveModel(string modelId)
        {
            return _activeModels.FirstOrDefault(x => x.Id == modelId);
        }

        private void Activate(QuestModel model)
        {
            _activeModels.Add(model);
            EmitSignal(nameof(QuestModelActivated), this, model.Id);

            if (model is QuestStartModel qsm)
            {
                EmitSignal(nameof(QuestStarted), this, qsm.Id);
                AdvanceFromModel(qsm);
            }
            else if (model is QuestStageModel qstm)
            {
                EmitSignal(nameof(QuestStageStarted), this, qstm.Id);
                AdvanceFromModel(qstm);
            }
            else if (model is QuestEventModel qem)
            {
                EmitSignal(nameof(QuestEventStarted), this, qem.Id);
                InitializeEvent(qem);
            }
            else if (model is QuestCompleteModel qcm)
            {
                AdvanceFromModel(qcm);
            }
        }

        private void AdvanceFromModel(QuestModel model)
        {
            _activeModels.Remove(model);
            EmitSignal(nameof(QuestModelDeactivated), this, model.Id);

            if (_activeModels.Count > 0)
            {
                // we still have models to process (most likely events)
                // so don't advance just yet
                return;
            }

            this.DisconnectAllSignals(GameEventDispatcher.Instance);

            if (model is QuestEventModel questEvent)
            {
                EmitSignal(nameof(QuestEventCompleted), this, model.Id);
            }
            else if (model is QuestCompleteModel questComplete)
            {
                EmitSignal(nameof(QuestCompleted), this, model.Id);
                QueueFree();
            }

            if (QuestSaveModel.RightConnections.ContainsKey(model.Id))
            {
                foreach (var rightConnectionId in QuestSaveModel.RightConnections[model.Id])
                {
                    var toModel = QuestSaveModel.IdToModelMap[rightConnectionId];
                    Activate(toModel);
                }
            }
        }

        private void InitializeEvent(QuestEventModel eventModel)
        {
            _eventProgress[eventModel.Id] = 0;
            switch (eventModel.EventId)
            {
                case GameEventDispatcher.PLAYER_INVENTORY_ITEM_UPDATED:
                    GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventPlayerInventoryItemUpdated), this, nameof(CheckInventoryItemUpdatedCompletion));
                    CheckInventoryItemUpdatedCompletion(eventModel.EventId, eventModel.ItemId);
                    break;
                case GameEventDispatcher.ENTITY_KILLED:
                    GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventEntityKilled), this, nameof(CheckEntityKilledCompletion));
                    break;
                case GameEventDispatcher.ITEM_TURNED_IN:
                    GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventItemTurnedIn), this, nameof(CheckTurnInCompletion));
                    break;
                case GameEventDispatcher.ENTITY_ENGAGED:
                    GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventEntityEngaged), this, nameof(CheckEntityEngagedCompletion));
                    break;
            }
        }

        private void CheckInventoryItemUpdatedCompletion(string eventGuid, string itemId)
        {
            var evt = _activeModels.Where(x => x is QuestEventModel qem && qem.EventId == eventGuid && qem.ItemId == itemId).Select(x => x as QuestEventModel).FirstOrDefault();
            if (evt != null && _eventProgress.ContainsKey(evt.Id))
            {
                var count = Mathf.Clamp(PlayerInventory.GetItemCount(itemId), 0, evt.Required);
                _eventProgress[evt.Id] = count;
                EmitSignal(nameof(QuestEventProgress), this, evt.Id);
                if (count == evt.Required)
                {
                    AdvanceFromModel(evt);
                }
            }
        }

        private void CheckEntityKilledCompletion(string eventGuid, string entityId)
        {
            var evt = _activeModels.Where(x => x is QuestEventModel qem && qem.EventId == eventGuid && qem.ItemId == entityId).Select(x => x as QuestEventModel).FirstOrDefault();
            if (evt != null && _eventProgress.ContainsKey(evt.Id))
            {
                _eventProgress[evt.Id]++;
                EmitSignal(nameof(QuestEventProgress), this, evt.Id);
                if (_eventProgress[evt.Id] == evt.Required)
                {
                    AdvanceFromModel(evt);
                }
            }
        }

        private void CheckTurnInCompletion(string eventGuid, string modelId, string itemId, int amount)
        {
            var evt = _activeModels.Where(x => x is QuestEventModel && x.Id == modelId).Select(x => x as QuestEventModel).FirstOrDefault();
            if (evt != null && _eventProgress.ContainsKey(evt.Id))
            {
                _eventProgress[evt.Id] += amount;
                if (_eventProgress[evt.Id] == evt.Required)
                {
                    AdvanceFromModel(evt);
                }
            }
        }

        private void CheckEntityEngagedCompletion(string eventGuid, string entityId)
        {
            var evt = _activeModels.Where(x => x is QuestEventModel qem && qem.EventId == eventGuid && qem.ItemId == entityId).FirstOrDefault();
            if (evt != null)
            {
                AdvanceFromModel(evt);
            }
        }
    }
}