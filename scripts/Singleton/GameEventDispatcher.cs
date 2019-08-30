using System.Collections.Generic;
using GameFeel.Component;
using Godot;

namespace GameFeel.Singleton
{
    public class GameEventDispatcher : Node
    {
        [Signal]
        public delegate void EventEntityKilled(string eventGuid, string entityGuid);
        [Signal]
        public delegate void EventPlayerInventoryItemUpdated(string eventGuid, string itemGuid);
        [Signal]
        public delegate void EventItemTurnedIn(string eventGuid, string modelId, string itemGuid, int amount);
        [Signal]
        public delegate void EventDialogueStarted(string eventGuid, DialogueComponent dialogueComponent);
        [Signal]
        public delegate void EventEntityEngaged(string eventGuid, string entityGuid);

        public const string PLAYER_INVENTORY_ITEM_UPDATED = "aaa35184-7b8d-5544-a642-722a842e6b27";
        public const string ENTITY_KILLED = "2e51c8d6-47ab-55aa-a274-66ff242365d7";
        public const string DIALOGUE_STARTED = "a5fc634c-b6bb-5976-ab80-44bc7b9f7318";
        public const string ITEM_TURNED_IN = "b9c562bc-d71c-5870-8c52-ea7e4ba5d81f";
        public const string ENTITY_ENGAGED = "ccef037b-eca4-5a83-b68e-5e0484084187";

        public static Dictionary<string, GameEvent> GameEventMapping { get; private set; } = new Dictionary<string, GameEvent>();
        public static GameEventDispatcher Instance { get; private set; }

        public class GameEvent
        {
            public string DisplayName { get; private set; }
            public string Id { get; private set; }

            public GameEvent(string id, string displayName)
            {
                Id = id;
                DisplayName = displayName;
            }
        }

        public override void _Ready()
        {
            Instance = this;
            GameEventMapping.Add(PLAYER_INVENTORY_ITEM_UPDATED, new GameEvent(PLAYER_INVENTORY_ITEM_UPDATED, nameof(PLAYER_INVENTORY_ITEM_UPDATED)));
            GameEventMapping.Add(ENTITY_KILLED, new GameEvent(ENTITY_KILLED, nameof(ENTITY_KILLED)));
            GameEventMapping.Add(ITEM_TURNED_IN, new GameEvent(ITEM_TURNED_IN, nameof(ITEM_TURNED_IN)));
            GameEventMapping.Add(DIALOGUE_STARTED, new GameEvent(DIALOGUE_STARTED, nameof(DIALOGUE_STARTED)));
            GameEventMapping.Add(ENTITY_ENGAGED, new GameEvent(ENTITY_ENGAGED, nameof(ENTITY_ENGAGED)));
        }

        public static void DispatchEntityKilledEvent(string entityGuid)
        {
            Instance.EmitSignal(nameof(EventEntityKilled), ENTITY_KILLED, entityGuid);
        }

        public static void DispatchPlayerInventoryItemUpdatedEvent(string itemGuid)
        {
            Instance.EmitSignal(nameof(EventPlayerInventoryItemUpdated), PLAYER_INVENTORY_ITEM_UPDATED, itemGuid);
        }

        public static void DispatchItemTurnedInEvent(string modelId, string itemGuid, int amount)
        {
            Instance.EmitSignal(nameof(EventItemTurnedIn), ITEM_TURNED_IN, modelId, itemGuid, amount);
        }

        public static void DispatchDialogueStartedEvent(DialogueComponent dialogueComponent)
        {
            Instance.EmitSignal(nameof(EventDialogueStarted), DIALOGUE_STARTED, dialogueComponent);
        }

        public static void DispatchEntityEngagedEvent(string entityGuid)
        {
            Instance.EmitSignal(nameof(EventEntityEngaged), ENTITY_ENGAGED, entityGuid);
        }
    }
}