using System.Collections.Generic;
using GameFeel.Data;
using GameFeel.GameObject.Loot;
using Godot;

namespace GameFeel.Singleton
{
    public class PlayerInventory : Node
    {
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
        }

        public static void AddItem(LootItem lootItem)
        {
            var item = new InventoryItem();
            item.Amount = 1;
            item.Icon = lootItem.IconTexture;
            item.Id = lootItem.ItemId;
            AddItem(item);
        }

        public static void AddItem(string itemId, Texture texture)
        {
            var item = new InventoryItem();
            item.Amount = 1;
            item.Icon = texture;
            item.Id = itemId;
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
    }
}