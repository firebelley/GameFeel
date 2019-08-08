using GameFeel.Resource;
using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;

namespace GameFeel.UI
{
    public class PlayerInventoryUI : CanvasLayer
    {
        private const string INPUT_INVENTORY = "inventory";
        private const string INPUT_DESELECT = "deselect";

        [Export]
        private NodePath _gridContainerPath;
        [Export]
        private NodePath _rootControlPath;

        private ResourcePreloader _resourcePreloader;
        private GridContainer _gridContainer;
        private Control _rootControl;
        private int _selectedIndex = -1;

        public override void _Ready()
        {
            this.SetNodesByDeclaredNodePaths();
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
            CreateInventoryCells();

            _rootControl.Visible = false;
            PlayerInventory.Instance.Connect(nameof(PlayerInventory.ItemAdded), this, nameof(OnItemAdded));
            _rootControl.Connect("visibility_changed", this, nameof(OnRootControlVisibilityChanged));
            _rootControl.Connect("gui_input", this, nameof(OnGuiInput));
        }

        public override void _UnhandledInput(InputEvent evt)
        {
            if (evt.IsActionPressed(INPUT_INVENTORY))
            {
                GetTree().SetInputAsHandled();
                _rootControl.Visible = !_rootControl.Visible;
            }
        }

        private void CreateInventoryCells()
        {
            for (int i = 0; i < PlayerInventory.Items.Count; i++)
            {
                var inventoryCell = _resourcePreloader.InstanceScene<InventoryCell>();
                _gridContainer.AddChild(inventoryCell);
                inventoryCell.Connect(nameof(InventoryCell.Selected), this, nameof(OnCellSelected), new Godot.Collections.Array() { i });
            }
        }

        private void ClearSelection()
        {
            Cursor.SetSecondaryTexture(null);
            _selectedIndex = -1;
        }

        private void CancelSelection()
        {
            if (_selectedIndex >= 0)
            {
                PlayerInventory.SwapIndices(_selectedIndex, _selectedIndex);
            }
            ClearSelection();
        }

        private void SwapIndices(int idx1, int idx2)
        {
            PlayerInventory.SwapIndices(idx1, idx2);
            ClearSelection();
        }

        private void SelectIndex(int idx)
        {
            var item = PlayerInventory.Items[idx];
            var cell = _gridContainer.GetChild<InventoryCell>(idx);
            _selectedIndex = idx;
            Cursor.SetSecondaryTexture(item?.Icon ?? null);
            cell.SetInventoryItem(null);
        }

        private void OnItemAdded(int idx)
        {
            var cell = _gridContainer.GetChild<InventoryCell>(idx);
            var item = PlayerInventory.Items[idx];
            cell.SetInventoryItem(item);
        }

        private void OnCellSelected(int idx)
        {
            if (_selectedIndex >= 0)
            {
                SwapIndices(_selectedIndex, idx);
            }
            else if (PlayerInventory.Items[idx] != null)
            {
                SelectIndex(idx);
            }
        }

        private void OnRootControlVisibilityChanged()
        {
            if (!_rootControl.Visible)
            {
                CancelSelection();
            }
        }

        private void OnGuiInput(InputEvent evt)
        {
            if (evt.IsActionPressed(INPUT_DESELECT))
            {
                _rootControl.AcceptEvent();
                CancelSelection();
            }
        }
    }
}