using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;

namespace GameFeel.UI
{
    public class PlayerInventoryUI : CanvasLayer
    {
        private const string INPUT_INVENTORY = "inventory";

        [Export]
        private NodePath _gridContainerPath;
        [Export]
        private NodePath _rootControlPath;

        private ResourcePreloader _resourcePreloader;
        private GridContainer _gridContainer;
        private Control _rootControl;

        public override void _Ready()
        {
            this.SetNodesByDeclaredNodePaths();
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
            CreateInventoryCells();

            _rootControl.Visible = false;
            PlayerInventory.Instance.Connect(nameof(PlayerInventory.ItemAdded), this, nameof(OnItemAdded));
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
            foreach (var item in PlayerInventory.Items)
            {
                var inventoryCell = _resourcePreloader.InstanceScene<InventoryCell>();
                _gridContainer.AddChild(inventoryCell);
            }
        }

        private void OnItemAdded(int idx)
        {
            var cell = _gridContainer.GetChild<InventoryCell>(idx);
            var item = PlayerInventory.Items[idx];
            cell.SetInventoryItem(item);
        }
    }
}