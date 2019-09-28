using System.Collections.Generic;
using GameFeel.Component;
using GameFeel.GameObject;
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
        [Signal]
        public delegate void EventPlayerDied(string eventGuid);
        [Signal]
        public delegate void EventPlayerInteract(string eventGuid);
        [Signal]
        public delegate void EventPlayerHealthChanged(string eventGuid, Player player);
        [Signal]
        public delegate void EventPlayerManaChanged(string eventGuid, Player player);
        [Signal]
        public delegate void EventPlayerCreated(string eventGuid, Player player);
        [Signal]
        public delegate void EventZoneChanged(string eventGuid, string zoneId);

        public const string PLAYER_INVENTORY_ITEM_UPDATED = "aaa35184-7b8d-5544-a642-722a842e6b27";
        public const string ENTITY_KILLED = "2e51c8d6-47ab-55aa-a274-66ff242365d7";
        public const string DIALOGUE_STARTED = "a5fc634c-b6bb-5976-ab80-44bc7b9f7318";
        public const string ITEM_TURNED_IN = "b9c562bc-d71c-5870-8c52-ea7e4ba5d81f";
        public const string ENTITY_ENGAGED = "ccef037b-eca4-5a83-b68e-5e0484084187";
        public const string PLAYER_DIED = "6d556c41-90d1-513f-a981-34f0d2cf404f";
        public const string PLAYER_INTERACT = "2a9af056-564c-53af-92b1-65395d9a08bd";
        public const string PLAYER_HEALTH_CHANGED = "abf010d8-2216-56fc-930b-58cabc83a533";
        public const string PLAYER_MANA_CHANGED = "7e5f57f6-58d8-5370-8930-062e7677a12a";
        public const string PLAYER_CREATED = "8d52e404-c184-5bdf-8dde-7a90a03a3cba";
        public const string ZONE_CHANGED = "2bda975e-24a2-5ee8-ba18-d00cf20160cb";

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
            GameEventMapping.Add(PLAYER_DIED, new GameEvent(PLAYER_DIED, nameof(PLAYER_DIED)));
            GameEventMapping.Add(PLAYER_INTERACT, new GameEvent(PLAYER_INTERACT, nameof(PLAYER_INTERACT)));
            GameEventMapping.Add(PLAYER_HEALTH_CHANGED, new GameEvent(PLAYER_HEALTH_CHANGED, nameof(PLAYER_HEALTH_CHANGED)));
            GameEventMapping.Add(PLAYER_MANA_CHANGED, new GameEvent(PLAYER_MANA_CHANGED, nameof(PLAYER_MANA_CHANGED)));
            GameEventMapping.Add(PLAYER_CREATED, new GameEvent(PLAYER_CREATED, nameof(PLAYER_CREATED)));
            GameEventMapping.Add(ZONE_CHANGED, new GameEvent(ZONE_CHANGED, nameof(ZONE_CHANGED)));
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

        public static void DispatchPlayerDiedEvent()
        {
            Instance.EmitSignal(nameof(EventPlayerDied), PLAYER_DIED);
        }

        public static void DispatchPlayerInteractEvent()
        {
            Instance.EmitSignal(nameof(EventPlayerInteract), PLAYER_INTERACT);
        }

        public static void DispatchPlayerHealthChangedEvent(Player player)
        {
            Instance.EmitSignal(nameof(EventPlayerHealthChanged), PLAYER_HEALTH_CHANGED, player);
        }

        public static void DispatchPlayerManaChangedEvent(Player player)
        {
            Instance.EmitSignal(nameof(EventPlayerManaChanged), PLAYER_MANA_CHANGED, player);
        }

        public static void DispatchPlayerCreatedEvent(Player player)
        {
            Instance.EmitSignal(nameof(EventPlayerCreated), PLAYER_CREATED, player);
        }

        public static void DispatchZoneChangedEvent(string zoneId)
        {
            Instance.EmitSignal(nameof(EventZoneChanged), ZONE_CHANGED, zoneId);
        }
    }
}