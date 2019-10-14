using GameFeel.Component;
using GameFeel.Resource;
using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;

namespace GameFeel.UI
{
    public class PlayerInventoryUI : ToggleUI
    {
        private const string INPUT_INVENTORY = "inventory";
        private const string INPUT_DESELECT = "deselect";
        private const string INPUT_SELECT = "select";
        private const string ANIM_BOUNCE_IN = "ControlBounceIn";

        [Export]
        private NodePath _gridContainerPath;
        [Export]
        private NodePath _animationPlayerPath;
        [Export]
        private NodePath _panelContainerPath;
        [Export]
        private NodePath _currencyLabelPath;
        [Export]
        private NodePath _inventoryPickupPath;
        [Export]
        private NodePath _inventoryPlacePath;
        [Export]
        private NodePath _inventoryClosePath;
        [Export]
        private NodePath _inventoryOpenPath;

        private ResourcePreloader _resourcePreloader;
        private GridContainer _gridContainer;
        private AnimationPlayer _animationPlayer;
        private Control _panelContainer;
        private Label _currencyLabel;
        private AudioStreamPlayerComponent _inventoryPickup;
        private AudioStreamPlayerComponent _inventoryPlace;
        private AudioStreamPlayerComponent _inventoryOpen;
        private AudioStreamPlayerComponent _inventoryClose;

        public override void _Ready()
        {
            base._Ready();
            this.SetNodesByDeclaredNodePaths();
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
            CreateInventoryCells();

            Visible = false;
            Close();
            PlayerInventory.Instance.Connect(nameof(PlayerInventory.ItemUpdated), this, nameof(OnItemUpdated));
            PlayerInventory.Instance.Connect(nameof(PlayerInventory.CurrencyChanged), this, nameof(OnCurrencyChanged));
            _panelContainer.Connect("resized", this, nameof(OnPanelResized));
        }

        public override void _UnhandledInput(InputEvent evt)
        {
            if (evt.IsActionPressed(INPUT_INVENTORY))
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

        protected override void Open()
        {
            base.Open();
            Show();
            if (_animationPlayer.IsPlaying())
            {
                _animationPlayer.Seek(0f, true);
            }
            _animationPlayer.Play(ANIM_BOUNCE_IN);
            Camera.ClearShift();
            _inventoryOpen.Play();
        }

        protected override void Deselect()
        {
            base.Deselect();
            CancelSelection();
        }

        protected override void Close()
        {
            base.Close();
            if (Visible)
            {
                _inventoryClose.Play();
            }
            Hide();
            CancelSelection();
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

        private void CancelSelection()
        {
            if (Cursor.DragFrom == Cursor.DragSource.INVENTORY && Cursor.DragIndex >= 0)
            {
                SwapIndices(Cursor.DragIndex, Cursor.DragIndex);
            }
        }

        private void SwapIndices(int idx1, int idx2)
        {
            PlayerInventory.SwapIndices(idx1, idx2);
            Cursor.ClearDragSelection();
        }

        private void SelectIndex(int idx)
        {
            var item = PlayerInventory.Items[idx];
            Cursor.Drag(Cursor.DragSource.INVENTORY, item, idx);
            ClearCell(idx);
        }

        private void OnItemUpdated(int idx)
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
            if (Cursor.DragIndex >= 0)
            {
                if (Cursor.DragFrom == Cursor.DragSource.INVENTORY)
                {
                    SwapIndices(Cursor.DragIndex, idx);
                    _inventoryPlace.Play();
                }
                else if (Cursor.DragFrom == Cursor.DragSource.EQUIPMENT && PlayerInventory.CanEquipInventoryItem(Cursor.DragIndex, idx))
                {
                    PlayerInventory.SwapEquipmentAndInventoryItem(Cursor.DragIndex, idx);
                    Cursor.ClearDragSelection();
                    _inventoryPlace.Play();
                }
            }
            else if (PlayerInventory.Items[idx] != null)
            {
                SelectIndex(idx);
                _inventoryPickup.Play();
            }
        }

        private void OnPanelResized()
        {
            _panelContainer.RectPivotOffset = _panelContainer.RectSize / 2f;
            Camera.ClearShift();
            Camera.AddShift(Main.UI_TO_GAME_DISPLAY_RATIO * _panelContainer.RectSize * Vector2.Right / 2f);
        }

        private void OnCurrencyChanged()
        {
            _currencyLabel.Text = PlayerInventory.PrimaryCurrency.ToString();
        }
    }
}