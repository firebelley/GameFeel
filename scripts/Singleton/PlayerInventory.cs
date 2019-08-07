using System.Collections.Generic;
using System.Linq;
using GameFeel.Data;
using GameFeel.GameObject.Loot;
using Godot;

namespace GameFeel.Singleton
{
    public class PlayerInventory : Node
    {
        private const int MAX_SIZE = 30;

        private static List<InventoryItem> _items = new List<InventoryItem>();

        public override void _Ready()
        {
            for (int i = 0; i < MAX_SIZE; i++)
            {
                _items.Add(null);
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
            var item = FindItem(inventoryItem.Id);
            if (item != null)
            {
                item.Amount += inventoryItem.Amount;
            }
            else
            {
                var idx = FindFirstNullIndex();
                if (idx >= 0)
                {
                    _items[idx] = inventoryItem;
                }
                else
                {
                    // throw some kind of full error here
                }
            }
        }

        public static InventoryItem FindItem(string itemId)
        {
            return _items.FirstOrDefault(x => x != null && x.Id == itemId);
        }

        public static int FindFirstNullIndex()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}