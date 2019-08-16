using System.Collections.Generic;
using System.Linq;
using GameFeel.Data.Model;
using GameFeel.Singleton;
using Godot;

namespace GameFeel.Resource
{
    public class Quest : Node
    {
        private QuestSaveModel _questSaveModel;
        private List<QuestEventModel> _trackedEvents = new List<QuestEventModel>();

        public override void _Ready()
        {

        }

        public void SetQuestModel(QuestSaveModel questSaveModel)
        {
            _questSaveModel = questSaveModel;
            BeginEventListen(_questSaveModel.Events[0]);
        }

        private void BeginEventListen(QuestEventModel eventModel)
        {
            switch (eventModel.EventId)
            {
                case GameEventDispatcher.ENTITY_KILLED:
                    break;
                case GameEventDispatcher.PLAYER_INVENTORY_ITEM_ADDED:
                    GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventPlayerInventoryItemAdded), this, nameof(OnInventoryItemAdded));
                    break;
            }
            _trackedEvents.Add(eventModel);
        }

        private void OnInventoryItemAdded(string eventGuid, string itemId)
        {
            // TODO: this won't work if another event looks for the same id but is not the inventory item added event
            if (_trackedEvents.Any(x => x.EventId == eventGuid && x.ItemId == itemId))
            {
                GD.Print("wooo");
            }
        }
    }
}