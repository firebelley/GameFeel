using System.Collections.Generic;
using System.Linq;
using GameFeel.Singleton;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestItemSelector : Button
    {
        [Signal]
        public delegate void ItemSelected(string id);

        [Export]
        private ItemType _itemType = ItemType.LOOT;

        private ItemList _itemList;
        private WindowDialog _windowDialog;
        private List<MetadataLoader.Metadata> _items;
        private Dictionary<string, MetadataLoader.Metadata> _sourceDict = new Dictionary<string, MetadataLoader.Metadata>();

        private enum ItemType
        {
            LOOT,
            ENTITY
        }

        public override void _Ready()
        {
            _itemList = GetNode<ItemList>("CanvasLayer/WindowDialog/ItemList");
            _windowDialog = GetNode<WindowDialog>("CanvasLayer/WindowDialog");

            switch (_itemType)
            {
                case ItemType.LOOT:
                    _sourceDict = MetadataLoader.LootItemIdToMetadata;
                    break;
                case ItemType.ENTITY:
                    _sourceDict = MetadataLoader.EntityIdToMetadata;
                    break;
            }

            _items = _sourceDict.Select(x => x.Value).OrderBy(x => x.DisplayName).ToList();
            foreach (var item in _items)
            {
                _itemList.AddItem(FormatButtonText(item.Id));
            }
            _itemList.Connect("item_selected", this, nameof(OnItemSelected));
            Connect("pressed", this, nameof(OnItemButtonPressed));
        }

        public void SetItemId(string id)
        {
            Text = FormatButtonText(id ?? string.Empty);
        }

        private string FormatButtonText(string id)
        {
            if (_sourceDict.ContainsKey(id))
            {
                return _sourceDict[id].DisplayName + " (" + id + ")";
            }
            return id;
        }

        private void OnItemButtonPressed()
        {
            _windowDialog.PopupCenteredRatio();
        }

        private void OnItemSelected(int idx)
        {
            _windowDialog.Hide();
            SetItemId(_items[idx].Id);
            EmitSignal(nameof(ItemSelected), _items[idx].Id);
        }
    }
}