using System.Collections.Generic;
using System.Linq;
using GameFeel.Data.Model;
using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;

namespace GameFeel.Resource
{
    public class Quest : Node
    {
        [Signal]
        public delegate void StageStarted();

        private QuestSaveModel _questSaveModel;
        private Dictionary<string, QuestModel> _idToModelMap;
        private List<QuestEventModel> _trackedEvents = new List<QuestEventModel>();
        private Stack<QuestStageModel> _stageStack = new Stack<QuestStageModel>();

        public override void _Ready()
        {
            GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventPlayerInventoryItemAdded), this, nameof(CheckInventoryItemAddedCompletion));
            GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventEntityKilled), this, nameof(CheckEntityKilledCompletion));
        }

        public void Start(QuestSaveModel questSaveModel)
        {
            _questSaveModel = questSaveModel;
            _idToModelMap = _questSaveModel.IdToModelMap;
            var start = _questSaveModel.Start;

            if (_questSaveModel.RightConnections.ContainsKey(start.Id))
            {
                foreach (var rightConnectionId in _questSaveModel.RightConnections[start.Id])
                {
                    var toModel = _idToModelMap[rightConnectionId];
                    if (toModel is QuestStageModel qsm)
                    {
                        StartStage(qsm);
                        break;
                    }
                }
            }
        }

        private void StartStage(QuestStageModel questStageModel)
        {
            ClearTrackedEvents();
            _stageStack.Push(questStageModel);

            if (_questSaveModel.RightConnections.ContainsKey(questStageModel.Id))
            {
                foreach (var rightConnectionId in _questSaveModel.RightConnections[questStageModel.Id])
                {
                    var toModel = _idToModelMap[rightConnectionId];
                    if (toModel is QuestEventModel qem)
                    {
                        TrackEvent(qem);
                    }
                }
            }
            EmitSignal(nameof(StageStarted));
        }

        private void TrackEvent(QuestEventModel eventModel)
        {
            _trackedEvents.Add(eventModel);
            switch (eventModel.EventId)
            {
                case GameEventDispatcher.PLAYER_INVENTORY_ITEM_ADDED:
                    CheckInventoryItemAddedCompletion(eventModel.EventId, eventModel.Id);
                    break;
                case GameEventDispatcher.ENTITY_KILLED:
                    CheckEntityKilledCompletion(eventModel.EventId, eventModel.Id);
                    break;
            }
        }

        private void ClearTrackedEvents()
        {
            _trackedEvents.Clear();
        }

        private void AttemptAdvance(QuestEventModel questEventModel)
        {
            GD.Print("holy moly!");
        }

        private void CheckInventoryItemAddedCompletion(string eventGuid, string itemId)
        {
            var evt = _trackedEvents.Find(x => x.EventId == eventGuid && x.ItemId == itemId);
            if (evt != null && PlayerInventory.GetItemCount(itemId) == evt.Required)
            {
                AttemptAdvance(evt);
            }
        }

        private void CheckEntityKilledCompletion(string eventGuid, string entityId)
        {
            if (_trackedEvents.Any(x => x.EventId == eventGuid && x.Id == entityId))
            {
                GD.Print("entity killed");
            }
        }
    }
}