using GameFeel.Singleton;
using Godot;

namespace GameFeel.Data
{
    public class InventoryItem
    {
        public string Id { get; set; }
        public Texture Icon { get; set; }
        public int Amount { get; set; } = 1;

        public static InventoryItem FromMetadata(MetadataLoader.Metadata resource)
        {
            var item = new InventoryItem();
            item.Icon = resource.Icon;
            item.Id = resource.Id;
            return item;
        }

        public static InventoryItem FromItemId(string id)
        {
            var metaData = MetadataLoader.LootItemIdToMetadata[id];
            return FromMetadata(metaData);
        }
    }
}