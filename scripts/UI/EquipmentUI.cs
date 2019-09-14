using GameFeel.Resource;
using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;

namespace GameFeel.UI
{
    public class EquipmentUI : ToggleUI
    {
        private const string INPUT_CHARACTER = "character";

        [Export]
        private NodePath _slot1Path;

        private InventoryCell _slot1;

        public override void _Ready()
        {
            base._Ready();
            this.SetNodesByDeclaredNodePaths();
            Close();

            _slot1.Connect(nameof(InventoryCell.Selected), this, nameof(OnCellSelected), new Godot.Collections.Array() { _slot1 });
        }

        public override void _UnhandledInput(InputEvent evt)
        {
            if (evt.IsActionPressed(INPUT_CHARACTER))
            {
                GetTree().SetInputAsHandled();
                if (Visible)
                {
                    Close();
                }
                else
                {
                    Open();
                }
            }
        }

        protected override void Close()
        {
            base.Close();
            Hide();
        }

        protected override void Open()
        {
            base.Open();
            Show();
        }

        private void OnCellSelected(InventoryCell inventoryCell)
        {
            if (Cursor.InventorySelectedIndex > -1)
            {
                var item = PlayerInventory.GetItemAtIndex(Cursor.InventorySelectedIndex);
                if (item != null)
                {
                    PlayerInventory.RemoveItemAtIndex(Cursor.InventorySelectedIndex, 1);
                    inventoryCell.SetInventoryItem(item);
                    Cursor.ClearInventorySelection();
                }
            }
        }
    }
}