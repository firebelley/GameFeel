using GameFeel.Data;
using Godot;

namespace GameFeel.UI
{
    public class InventoryCell : Control
    {
        private const string INPUT_SELECT = "select";
        private const string ANIM_HOVER = "hover";

        [Signal]
        public delegate void Selected();

        private TextureRect _backgroundTextureRect;
        private TextureRect _foregroundTextureRect;
        private AnimationPlayer _animationPlayer;
        private Label _countLabel;
        private InventoryItem _inventoryItem;
        private bool _mouseHovered;

        public override void _Ready()
        {
            _backgroundTextureRect = GetNode<TextureRect>("TextureRectBackground");
            _foregroundTextureRect = GetNode<TextureRect>("TextureRectForeground");
            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _countLabel = GetNode<Label>("CountLabel");

            Connect("gui_input", this, nameof(OnGuiInput));
            Connect("mouse_entered", this, nameof(OnMouseEntered));
            Connect("mouse_exited", this, nameof(OnMouseExited));
            Connect("visibility_changed", this, nameof(OnVisibilityChanged));
        }

        public void SetInventoryItem(InventoryItem inventoryItem)
        {
            _inventoryItem = inventoryItem;
            if (_inventoryItem == null)
            {
                Clear();
                return;
            }

            _backgroundTextureRect.Texture = _inventoryItem.Icon;
            if (_inventoryItem.Amount > 1)
            {
                _countLabel.Text = _inventoryItem.Amount.ToString();
            }

            if (_mouseHovered)
            {
                TooltipUI.ShowInventoryItemTooltip(_inventoryItem.Id);
            }
        }

        private void Clear()
        {
            _backgroundTextureRect.Texture = null;
            _countLabel.Text = string.Empty;
        }

        private void StopAnimation()
        {
            if (_animationPlayer.IsPlaying())
            {
                _animationPlayer.Stop();
                _animationPlayer.Seek(0f, true);
            }
        }

        private void OnGuiInput(InputEvent evt)
        {
            if (evt.IsActionPressed(INPUT_SELECT))
            {
                AcceptEvent();
                EmitSignal(nameof(Selected));
            }
        }

        private void OnMouseEntered()
        {
            _mouseHovered = true;
            _animationPlayer.Play(ANIM_HOVER);
            if (_inventoryItem != null)
            {
                TooltipUI.ShowInventoryItemTooltip(_inventoryItem.Id);
            }
        }

        private void OnMouseExited()
        {
            _mouseHovered = false;
            StopAnimation();
            TooltipUI.HideTooltip();
        }

        private void OnVisibilityChanged()
        {
            if (!IsVisibleInTree())
            {
                StopAnimation();
            }
        }
    }
}