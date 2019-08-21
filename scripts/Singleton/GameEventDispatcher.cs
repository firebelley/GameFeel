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
        public delegate void EventPlayerInventoryItemAdded(string eventGuid, string itemGuid);
        [Signal]
        public delegate void EventDialogueStarted(string eventGuid, DialogueComponent dialogueComponent);

        public const string PLAYER_INVENTORY_ITEM_ADDED = "aaa35184-7b8d-5544-a642-722a842e6b27";
        public const string ENTITY_KILLED = "2e51c8d6-47ab-55aa-a274-66ff242365d7";
        public const string DIALOGUE_STARTED = "a5fc634c-b6bb-5976-ab80-44bc7b9f7318";

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
            GameEventMapping.Add(PLAYER_INVENTORY_ITEM_ADDED, new GameEvent(PLAYER_INVENTORY_ITEM_ADDED, nameof(PLAYER_INVENTORY_ITEM_ADDED)));
            GameEventMapping.Add(ENTITY_KILLED, new GameEvent(ENTITY_KILLED, nameof(ENTITY_KILLED)));
        }

        public static void DispatchEntityKilledEvent(string entityGuid)
        {
            Instance.EmitSignal(nameof(EventEntityKilled), ENTITY_KILLED, entityGuid);
        }

        public static void DispatchPlayerInventoryItemAddedEvent(string itemGuid)
        {
            Instance.EmitSignal(nameof(EventPlayerInventoryItemAdded), PLAYER_INVENTORY_ITEM_ADDED, itemGuid);
        }

        public static void DispatchDialogueStartedEvent(DialogueComponent dialogueComponent)
        {
            Instance.EmitSignal(nameof(EventDialogueStarted), DIALOGUE_STARTED, dialogueComponent);
        }
    }
}