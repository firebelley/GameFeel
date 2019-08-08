using GameFeel.Data;
using Godot;

namespace GameFeel.UI
{
    public class InventoryCell : Control
    {
        [Signal]
        public delegate void Selected();

        private const string INPUT_SELECT = "select";
        private TextureRect _backgroundTextureRect;
        private TextureRect _foregroundTextureRect;
        private Label _countLabel;

        public override void _Ready()
        {
            _backgroundTextureRect = GetNode<TextureRect>("TextureRectBackground");
            _foregroundTextureRect = GetNode<TextureRect>("TextureRectForeground");
            _countLabel = GetNode<Label>("CountLabel");

            Connect("gui_input", this, nameof(OnGuiInput));
        }

        public void SetInventoryItem(InventoryItem inventoryItem)
        {
            if (inventoryItem == null)
            {
                Clear();
                return;
            }
            _backgroundTextureRect.Texture = inventoryItem.Icon;
            if (inventoryItem.Amount > 1)
            {
                _countLabel.Text = inventoryItem.Amount.ToString();
            }
        }

        private void Clear()
        {
            _backgroundTextureRect.Texture = null;
            _countLabel.Text = string.Empty;
        }

        private void OnGuiInput(InputEvent evt)
        {
            if (evt.IsActionPressed(INPUT_SELECT))
            {
                AcceptEvent();
                EmitSignal(nameof(Selected));
            }
        }
    }
}