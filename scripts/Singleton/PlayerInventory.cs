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

        private const int MAX_SIZE = 25;

        public static PlayerInventory Instance { get; private set; }
        public static List<InventoryItem> Items { get; private set; } = new List<InventoryItem>();

        public override void _Ready()
        {
            Instance = this;
            for (int i = 0; i < MAX_SIZE; i++)
            {
                Items.Add(null);
            }
            Connect(nameof(ItemAdded), this, nameof(OnItemAdded));
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
            if (!MetadataLoader.LootItemIdToInfo.ContainsKey(itemId))
            {
                Logger.Error("No item with id " + itemId + " was loaded");
                return;
            }
            var info = MetadataLoader.LootItemIdToInfo[itemId];
            var lootItem = (GD.Load(info.ResourcePath) as PackedScene).Instance() as LootItem;

            var item = new InventoryItem();
            item.Amount = amount;
            item.Icon = lootItem.Icon;
            item.Id = itemId;

            lootItem.QueueFree();
            AddItem(item);
        }

        public static void AddItem(InventoryItem inventoryItem)
        {
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
            Instance.EmitSignal(nameof(ItemAdded), idx1);
            Instance.EmitSignal(nameof(ItemAdded), idx2);
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

        private void OnItemAdded(int idx)
        {
            // TODO: remove once signals are resolved
            if (Items[idx] != null)
            {
                GameEventDispatcher.DispatchPlayerInventoryItemAddedEvent(Items[idx].Id);
            }
        }
    }
}