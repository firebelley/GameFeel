using System.Collections.Generic;
using GameFeel.Data;
using GameFeel.GameObject.Loot;
using Godot;
using GodotTools.Util;

namespace GameFeel.Singleton
{
    public class PlayerInventory : Node
    {
        // TODO: make new signal for cell updating and item adding
        [Signal]
        public delegate void ItemAdded(int idx);
        [Signal]
        public delegate void ItemUpdated(int idx);
        [Signal]
        public delegate void CurrencyChanged();

        private const int MAX_SIZE = 25;

        public static PlayerInventory Instance { get; private set; }
        public static List<InventoryItem> Items { get; private set; } = new List<InventoryItem>();
        public static int PrimaryCurrency
        {
            get
            {
                return _primaryCurrency;
            }
            set
            {
                _primaryCurrency = value;
                Instance?.EmitSignal(nameof(CurrencyChanged));
            }
        }
        private static int _primaryCurrency;

        public override void _Ready()
        {
            Instance = this;
            for (int i = 0; i < MAX_SIZE; i++)
            {
                Items.Add(null);
            }
            Connect(nameof(ItemAdded), this, nameof(OnItemAdded));
            GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventItemTurnedIn), this, nameof(OnItemTurnedInEvent));
        }

        public static InventoryItem InventoryItemFromLootMetadata(MetadataLoader.Metadata resource)
        {
            var item = new InventoryItem();
            item.Icon = resource.Icon;
            item.Id = resource.Id;
            return item;
        }

        public static void AddItem(LootItem lootItem)
        {
            var item = new InventoryItem();
            item.Amount = 1;
            item.Icon = lootItem.Icon;
            item.Id = lootItem.Id;
            AddItem(item);
        }

        public static void AddItem(string itemId, int amount)
        {
            var metaData = MetadataLoader.LootItemIdToMetadata[itemId];
            var item = InventoryItemFromLootMetadata(metaData);
            item.Amount = amount;
            AddItem(item);
        }

        public static void AddItem(InventoryItem inventoryItem)
        {
            if (inventoryItem.Id == MetadataLoader.PRIMARY_CURRENCY_ID)
            {
                PrimaryCurrency += inventoryItem.Amount;
                return;
            }

            var itemIndex = FindItemIndex(inventoryItem.Id);
            if (itemIndex >= 0)
            {
                Items[itemIndex].Amount += inventoryItem.Amount;
                Instance.EmitSignal(nameof(ItemAdded), itemIndex);
            }
            else
            {
                var idx = FindFirstNullIndex();
                if (idx >= 0)
                {
                    Items[idx] = inventoryItem;
                    Instance.EmitSignal(nameof(ItemAdded), idx);
                }
                else
                {
                    // TODO: inventory is full
                    // throw some kind of full error here
                }
            }
        }

        public static void SwapIndices(int idx1, int idx2)
        {
            var val1 = Items[idx1];
            var val2 = Items[idx2];
            Items[idx1] = val2;
            Items[idx2] = val1;
            Instance.EmitSignal(nameof(ItemUpdated), idx1);
            Instance.EmitSignal(nameof(ItemUpdated), idx2);
        }

        public static int FindItemIndex(string itemId)
        {
            return Items.FindIndex(x => x != null && x.Id == itemId);
        }

        public static int FindFirstNullIndex()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int GetItemCount(string itemId)
        {
            var idx = FindItemIndex(itemId);
            if (idx < 0)
            {
                return 0;
            }
            return Items[idx].Amount;
        }

        public static void RemoveItem(string itemId, int amount)
        {
            var idx = FindItemIndex(itemId);
            if (idx < 0)
            {
                Logger.Error("Attempted to remove item from player inventory that did not exist " + itemId);
                return;
            }
            Items[idx].Amount -= amount;
            if (Items[idx].Amount <= 0)
            {
                if (Items[idx].Amount < 0)
                {
                    Logger.Error("Player inventory item amount larger than requested removal amount item id " + itemId + " amount " + amount);
                }
                Items[idx] = null;
            }
            Instance.EmitSignal(nameof(ItemUpdated), idx);
        }

        private void OnItemAdded(int idx)
        {
            GameEventDispatcher.DispatchPlayerInventoryItemAddedEvent(Items[idx].Id);
        }

        private void OnItemTurnedInEvent(string eventGuid, string modelId, string itemGuid, int amount)
        {
            RemoveItem(itemGuid, amount);
        }
    }
}