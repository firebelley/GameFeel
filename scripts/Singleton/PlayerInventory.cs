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
        public delegate void ItemUpdated(int idx);
        [Signal]
        public delegate void ItemCleared(string itemId);
        [Signal]
        public delegate void CurrencyChanged();
        [Signal]
        public delegate void ItemEquipped(Equipment equipment);
        [Signal]
        public delegate void EquipmentUpdated(int slotIdx);

        private const int MAX_SIZE = 25;
        private const int EQUIPMENT_SLOT_COUNT = 2;

        public static PlayerInventory Instance { get; private set; }
        public static List<InventoryItem> Items { get; private set; } = new List<InventoryItem>();
        public static InventoryItem[] EquipmentSlots { get; private set; } = new InventoryItem[EQUIPMENT_SLOT_COUNT];

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
            Connect(nameof(ItemUpdated), this, nameof(OnItemUpdated));
            Connect(nameof(ItemCleared), this, nameof(OnItemCleared));
            GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventItemTurnedIn), this, nameof(OnItemTurnedInEvent));
        }

        public static void AddItem(LootItem lootItem)
        {
            var item = new InventoryItem();
            item.Icon = lootItem.Icon;
            item.Id = lootItem.Id;
            AddItem(item);
        }

        public static void AddItem(string itemId, int amount)
        {
            var metaData = MetadataLoader.LootItemIdToMetadata[itemId];
            var item = InventoryItem.FromMetadata(metaData);
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
                Instance.EmitSignal(nameof(ItemUpdated), itemIndex);
            }
            else
            {
                var idx = FindFirstNullIndex();
                if (idx >= 0)
                {
                    Items[idx] = inventoryItem;
                    Instance.EmitSignal(nameof(ItemUpdated), idx);
                }
                else
                {
                    // TODO: inventory is full
                    // throw some kind of full error here
                    Logger.Error("Inventory was full, could not add item with id " + inventoryItem.Id);
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
            RemoveItemAtIndex(idx, amount);
        }

        public static InventoryItem GetItemAtIndex(int idx)
        {
            if (idx < Items.Count)
            {
                return Items[idx];
            }
            return null;
        }

        public static void RemoveItemAtIndex(int idx, int amount)
        {
            if (idx < 0 || idx >= Items.Count)
            {
                Logger.Error("Tried to remove an item index which was out of the array bounds");
                return;
            }

            Items[idx].Amount -= amount;
            if (Items[idx].Amount <= 0)
            {
                if (Items[idx].Amount < 0)
                {
                    Logger.Error("Player inventory item amount larger than requested removal amount item id " + Items[idx].Id + " amount " + amount);
                }
                var id = Items[idx].Id;
                Items[idx] = null;
                Instance.EmitSignal(nameof(ItemCleared), id);
            }
            Instance.EmitSignal(nameof(ItemUpdated), idx);
        }

        public static bool IsItemEquippable(string itemId)
        {
            return MetadataLoader.LootItemIdToEquipmentMetadata.ContainsKey(itemId);
        }

        public static bool ItemCanBeSlotted(string itemId, int slotIdx)
        {
            if (IsItemEquippable(itemId))
            {
                var equipmentMetadata = MetadataLoader.LootItemIdToEquipmentMetadata[itemId];
                return equipmentMetadata.SlotIndex == slotIdx;
            }
            return false;
        }

        public static void EquipInventoryItem(string itemId, int slot)
        {
            if (slot >= EquipmentSlots.Length)
            {
                Logger.Error("Tried to equip item out of bounds of array");
                return;
            }

            if (ItemCanBeSlotted(itemId, slot))
            {
                var itemIdx = FindItemIndex(itemId);
                if (itemIdx >= 0)
                {
                    var equipmentMetadata = MetadataLoader.LootItemIdToEquipmentMetadata[itemId];
                    RemoveItemAtIndex(itemIdx, 1);
                    if (EquipmentSlots[slot] != null)
                    {
                        AddItem(EquipmentSlots[slot].Id, 1);
                    }
                    EquipmentSlots[slot] = InventoryItem.FromMetadata(equipmentMetadata);
                    Instance.EmitSignal(nameof(EquipmentUpdated), slot);

                    var equipmentScene = GD.Load(equipmentMetadata.ResourcePath) as PackedScene;
                    var equipment = equipmentScene.Instance() as Equipment;
                    Instance.EmitSignal(nameof(ItemEquipped), equipment);
                }
            }
        }

        public static void SwapEquipmentAndInventoryItems(int slotIdx, int itemIdx)
        {
            if (itemIdx >= Items.Count || itemIdx < 0 || slotIdx < 0 || slotIdx >= EQUIPMENT_SLOT_COUNT)
            {
                Logger.Error("Tried to perform an out of bounds operation");
                return;
            }

            var item = Items[itemIdx];
            var equipment = EquipmentSlots[slotIdx];
            if (equipment == null)
            {
                return;
            }
            if (item != null && ItemCanBeSlotted(item.Id, slotIdx))
            {
                EquipInventoryItem(item.Id, slotIdx);
            }
        }

        private void OnItemUpdated(int idx)
        {
            if (Items[idx] != null)
            {
                GameEventDispatcher.DispatchPlayerInventoryItemUpdatedEvent(Items[idx].Id);
            }
        }

        private void OnItemCleared(string itemId)
        {
            GameEventDispatcher.DispatchPlayerInventoryItemUpdatedEvent(itemId);
        }

        private void OnItemTurnedInEvent(string eventGuid, string modelId, string itemGuid, int amount)
        {
            RemoveItem(itemGuid, amount);

            var quest = QuestTracker.GetActiveQuestContainingModelId(modelId);
            foreach (var reward in quest.GetRewards(modelId))
            {
                AddItem(reward.ItemId, reward.Amount);
            }
        }
    }
}