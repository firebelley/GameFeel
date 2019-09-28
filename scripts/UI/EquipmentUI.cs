using GameFeel.Data;
using GameFeel.Resource;
using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;

namespace GameFeel.UI
{
    public class EquipmentUI : ToggleUI
    {
        private const string INPUT_CHARACTER = "character";
        private const string ANIM_BOUNCE_IN = "ControlBounceIn";

        [Export]
        private NodePath _slot1Path;
        [Export]
        private NodePath _slot2Path;
        [Export]
        private NodePath _animationPlayerPath;
        [Export]
        private NodePath _panelContainerPath;

        private InventoryCell[] _slots = new InventoryCell[2];
        private InventoryCell _slot1;
        private InventoryCell _slot2;
        private AnimationPlayer _animationPlayer;
        private PanelContainer _panelContainer;

        public override void _Ready()
        {
            base._Ready();
            this.SetNodesByDeclaredNodePaths();
            Close();

            _animationPlayer = GetNode<AnimationPlayer>(_animationPlayerPath);

            _slots[0] = _slot1;
            _slots[1] = _slot2;

            for (int i = 0; i < _slots.Length; i++)
            {
                _slots[i].Connect(nameof(InventoryCell.Selected), this, nameof(OnCellSelected), new Godot.Collections.Array() { _slots[i], i });
            }

            _panelContainer.Connect("resized", this, nameof(OnPanelResized));
            PlayerInventory.Instance.Connect(nameof(PlayerInventory.EquipmentUpdated), this, nameof(OnEquipmentUpdated));
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
            CancelSelection();
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
        }

        protected override void Deselect()
        {
            base.Deselect();
            CancelSelection();
        }

        private void CancelSelection()
        {
            if (Cursor.DragFrom == Cursor.DragSource.EQUIPMENT && Cursor.DragIndex >= 0)
            {
                _slots[Cursor.DragIndex].SetInventoryItem(Cursor.Dragging);
                Cursor.ClearDragSelection();
            }
        }

        private void OnCellSelected(InventoryCell inventoryCell, int slotIdx)
        {
            if (Cursor.DragFrom == Cursor.DragSource.INVENTORY && Cursor.DragIndex > -1)
            {
                var item = PlayerInventory.GetItemAtIndex(Cursor.DragIndex);
                if (item != null && PlayerInventory.ItemCanBeSlotted(item.Id, slotIdx))
                {
                    var swapItem = InventoryItem.FromItemId(item.Id);
                    PlayerInventory.EquipInventoryItem(Cursor.DragIndex, slotIdx);
                    Cursor.ClearDragSelection();
                }
            }
            else if (Cursor.DragFrom == Cursor.DragSource.EQUIPMENT && Cursor.DragIndex > -1)
            {
                if (Cursor.DragIndex == slotIdx)
                {
                    inventoryCell.SetInventoryItem(Cursor.Dragging);
                    Cursor.ClearDragSelection();
                }
            }
            else if (PlayerInventory.EquipmentSlots[slotIdx] != null)
            {
                Cursor.Drag(Cursor.DragSource.EQUIPMENT, PlayerInventory.EquipmentSlots[slotIdx], slotIdx);
                inventoryCell.SetInventoryItem(null);
            }
        }

        private void OnPanelResized()
        {
            _panelContainer.RectPivotOffset = _panelContainer.RectSize / 2f;
        }

        private void OnEquipmentUpdated(int slotIdx)
        {
            _slots[slotIdx].SetInventoryItem(PlayerInventory.EquipmentSlots[slotIdx]);
        }
    }
}