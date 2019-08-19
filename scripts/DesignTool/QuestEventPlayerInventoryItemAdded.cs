using System.Collections.Generic;
using System.Linq;
using GameFeel.Singleton;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestEventPlayerInventoryItemAdded : QuestEventNode
    {
        private Button _itemButton;
        private SpinBox _requiredSpinBox;
        private ItemList _itemList;
        private WindowDialog _windowDialog;
        private List<KeyValuePair<string, string>> _items;

        public override void _Ready()
        {
            base._Ready();
            Model.EventId = GameEventDispatcher.PLAYER_INVENTORY_ITEM_ADDED;

            _itemButton = GetNode<Button>("VBoxContainer/HBoxContainer/Button");
            _requiredSpinBox = GetNode<SpinBox>("VBoxContainer/HBoxContainer2/SpinBox");
            _itemList = GetNode<ItemList>("CanvasLayer/WindowDialog/ItemList");
            _windowDialog = GetNode<WindowDialog>("CanvasLayer/WindowDialog");

            _itemButton.Connect("pressed", this, nameof(OnItemButtonPressed));
            _requiredSpinBox.Connect("value_changed", this, nameof(OnRequiredChanged));

            _items = QuestDesigner.ItemIdToDisplayName.OrderBy(x => x.Value).ToList();
            foreach (var item in _items)
            {
                _itemList.AddItem(FormatButtonText(item.Key));
            }
            _itemList.Connect("item_selected", this, nameof(OnItemSelected));
        }

        protected override void UpdateControls()
        {
            base.UpdateControls();
            _itemButton.Text = FormatButtonText(Model.ItemId ?? string.Empty);
            _requiredSpinBox.Value = Model.Required;
        }

        private string FormatButtonText(string id)
        {
            if (QuestDesigner.ItemIdToDisplayName.ContainsKey(id))
            {
                return QuestDesigner.ItemIdToDisplayName[id] + " (" + id + ")";
            }
            return id;
        }

        private void OnItemButtonPressed()
        {
            _windowDialog.PopupCenteredRatio();
        }

        private void OnRequiredChanged(float value)
        {
            Model.Required = (int) value;
        }

        private void OnItemSelected(int idx)
        {
            _windowDialog.Hide();
            Model.ItemId = _items[idx].Key;
            UpdateControls();
        }
    }
}