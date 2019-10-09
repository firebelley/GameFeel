using System.Collections.Generic;
using System.Linq;
using GameFeel.Data.Model;
using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;

namespace GameFeel.Resource
{
    public class Quest
    {
        public MetadataLoader.QuestMetadata Metadata { get; private set; }
        private QuestSaveModel _questSaveModel;
        private Dictionary<string, QuestModel> _idToModelMap;
        private HashSet<QuestModel> _activeModels = new HashSet<QuestModel>();
        private Dictionary<string, int> _eventProgress = new Dictionary<string, int>();

        public Quest(string questPath)
        {
            Metadata = MetadataLoader.QuestFileToMetadata[questPath];
            _questSaveModel = Metadata.QuestSaveModel;
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

        public List<QuestRewardModel> GetRewards(string modelId)
        {
            if (_questSaveModel.RightConnections.ContainsKey(modelId))
            {
                return _questSaveModel.RightConnections[modelId]
                    .Select(x => _idToModelMap[x] as QuestRewardModel)
                    .Where(x => x != null)
                    .ToList();
            }
            return new List<QuestRewardModel>();
        }

        public IEnumerable<QuestEventModel> GetActiveQuestEventModels(string eventGuid)
        {
            return _activeModels.Select(x => x as QuestEventModel).Where(x => x != null && x.EventId == eventGuid);
        }

        public void Start()
        {
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

        public void SetEventProgress(QuestEventModel eventModel, int amount)
        {
            _eventProgress[eventModel.Id] = Mathf.Clamp(amount, 0, eventModel.Required);
        }

        public void IncrementEventProgress(QuestEventModel eventModel)
        {
            IncrementEventProgress(eventModel, 1);
        }

        public void IncrementEventProgress(QuestEventModel eventModel, int amount)
        {
            _eventProgress[eventModel.Id] = Mathf.Clamp(_eventProgress[eventModel.Id] + amount, 0, eventModel.Required);
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
                QuestTracker.Instance.EmitSignal(nameof(QuestTracker.QuestEventStarted), Metadata.ResourcePath, model.Id);
            }
            else if (model is QuestCompleteModel qcm)
            {
                Complete(qcm);
            }
            EmitSignal(nameof(QuestModelActivated), this, model.Id);
        }

        public void AdvanceFromModel(QuestModel model)
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

        private void Complete(QuestCompleteModel questComplete)
        {
            EmitSignal(nameof(QuestCompleted), this, questComplete.Id);
        }
    }
}