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
        private const string INPUT_SELECT = "select";
        private const string ANIM_BOUNCE_IN = "BounceIn";

        [Export]
        private NodePath _gridContainerPath;
        [Export]
        private NodePath _rootControlPath;
        [Export]
        private NodePath _animationPlayerPath;
        [Export]
        private NodePath _panelContainerPath;

        private ResourcePreloader _resourcePreloader;
        private GridContainer _gridContainer;
        private AnimationPlayer _animationPlayer;
        private Control _panelContainer;
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
            _panelContainer.Connect("resized", this, nameof(OnPanelResized));
            _panelContainer.Connect("gui_input", this, nameof(OnPanelGuiInput));
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
            _selectedIndex = idx;
            Cursor.SetSecondaryTexture(item?.Icon ?? null);
            ClearCell(idx);
        }

        private void OnItemAdded(int idx)
        {
            ClearCell(idx);
            var cell = _gridContainer.GetChild<InventoryCell>(idx);
            var item = PlayerInventory.Items[idx];
            cell.SetInventoryItem(item);
        }

        private void ClearCell(int idx)
        {
            var cell = _gridContainer.GetChild<InventoryCell>(idx);
            cell.SetInventoryItem(null);
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
            if (!_rootControl.IsVisibleInTree())
            {
                CancelSelection();
            }
            else
            {
                if (_animationPlayer.IsPlaying())
                {
                    _animationPlayer.Seek(0f, true);
                }
                _animationPlayer.Play(ANIM_BOUNCE_IN);
                Camera.ClearShift();
            }
        }

        private void OnGuiInput(InputEvent evt)
        {
            if (evt.IsActionPressed(INPUT_DESELECT))
            {
                _rootControl.AcceptEvent();
                CancelSelection();
            }
            else if (evt.IsActionPressed(INPUT_SELECT))
            {
                _rootControl.Visible = false;
            }
        }

        private void OnPanelGuiInput(InputEvent evt)
        {
            if (evt.IsActionPressed(INPUT_SELECT))
            {
                _panelContainer.AcceptEvent();
            }
        }

        private void OnPanelResized()
        {
            _panelContainer.RectPivotOffset = _panelContainer.RectSize / 2f;
            Camera.ClearShift();
            Camera.AddShift(_panelContainer.RectSize * Vector2.Right / 2f);
        }
    }
}