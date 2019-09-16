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
            _slot1.Connect(nameof(InventoryCell.Selected), this, nameof(OnCellSelected), new Godot.Collections.Array() { _slot1, 0 });
            _slot2.Connect(nameof(InventoryCell.Selected), this, nameof(OnCellSelected), new Godot.Collections.Array() { _slot2, 1 });
            _panelContainer.Connect("resized", this, nameof(OnPanelResized));
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
            if (_animationPlayer.IsPlaying())
            {
                _animationPlayer.Seek(0f, true);
            }
            _animationPlayer.Play(ANIM_BOUNCE_IN);
        }

        private void OnCellSelected(InventoryCell inventoryCell, int slotIdx)
        {
            if (Cursor.DragIndex > -1)
            {
                var item = PlayerInventory.GetItemAtIndex(Cursor.DragIndex);
                if (item != null && PlayerInventory.ItemCanBeSlotted(item.Id, slotIdx))
                {
                    var swapItem = InventoryItem.FromItemId(item.Id);
                    PlayerInventory.EquipInventoryItem(item.Id, slotIdx);
                    inventoryCell.SetInventoryItem(swapItem);
                    Cursor.ClearDragSelection();
                }
            }
            else
            {

            }
        }

        private void OnPanelResized()
        {
            _panelContainer.RectPivotOffset = _panelContainer.RectSize / 2f;
        }
    }
}