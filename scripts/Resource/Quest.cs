using System.Collections.Generic;
using System.Linq;
using GameFeel.Data.Model;
using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;
using GodotTools.Util;

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

        private QuestSaveModel _questSaveModel;
        private Dictionary<string, QuestModel> _idToModelMap;
        private HashSet<QuestModel> _activeModels = new HashSet<QuestModel>();
        private Dictionary<string, int> _eventProgress = new Dictionary<string, int>();

        public override void _Ready()
        {

        }

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

        public void SetModel(QuestSaveModel questSaveModel)
        {
            _questSaveModel = questSaveModel;
            _idToModelMap = _questSaveModel.IdToModelMap;
        }

        public void Start()
        {
            if (_questSaveModel == null)
            {
                Logger.Error("No quest save model set before starting!");
                return;
            }
            Activate(_questSaveModel.Start);
        }

        public QuestSaveModel GetQuestSaveModel()
        {
            return _questSaveModel;
        }

        public bool ContainsModelId(string modelId)
        {
            return _idToModelMap.ContainsKey(modelId);
        }

        public QuestModel GetQuestModel(string modelId)
        {
            return _idToModelMap[modelId];
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
            if (model is QuestStartModel qsm)
            {
                AdvanceFromModel(qsm);
            }
            else if (model is QuestStageModel qstm)
            {
                AdvanceFromModel(qstm);
            }
            else if (model is QuestEventModel qem)
            {
                EmitSignal(nameof(QuestEventStarted), this, qem.Id);
                InitializeEvent(qem);
            }
            else if (model is QuestCompleteModel qcm)
            {
                Complete(qcm);
            }
            EmitSignal(nameof(QuestModelActivated), this, model.Id);
        }

        private void AdvanceFromModel(QuestModel model)
        {
            _activeModels.Remove(model);

            if (_activeModels.Count > 0)
            {
                // we still have models to process (most likely events)
                // so don't advance just yet
                return;
            }

            this.DisconnectAllSignals(GameEventDispatcher.Instance);

            if (model is QuestStartModel questStart)
            {
                EmitSignal(nameof(QuestStarted), this, model.Id);
            }
            else if (model is QuestEventModel questEvent)
            {
                EmitSignal(nameof(QuestEventCompleted), this, model.Id);
            }
            else if (model is QuestStageModel questStage)
            {
                EmitSignal(nameof(QuestStageStarted), this, model.Id);
            }

            if (_questSaveModel.RightConnections.ContainsKey(model.Id))
            {
                foreach (var rightConnectionId in _questSaveModel.RightConnections[model.Id])
                {
                    var toModel = _idToModelMap[rightConnectionId];
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

        private void Complete(QuestCompleteModel questComplete)
        {
            PlayerInventory.AddItem(questComplete.RewardItemId, questComplete.RewardItemAmount);
            EmitSignal(nameof(QuestCompleted), this, questComplete.Id);
            QueueFree();
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