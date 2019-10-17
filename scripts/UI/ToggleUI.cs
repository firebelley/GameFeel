using Godot;

namespace GameFeel.UI
{
    public class ToggleUI : Control
    {
        [Signal]
        public delegate void Opened(ToggleUI toggleUI);
        [Signal]
        public delegate void Closed(ToggleUI toggleUI);

        public override void _Ready()
        {
            if (Owner is UI ui)
            {
                ui.Connect(nameof(UI.CloseRequested), this, nameof(OnCloseRequested));
                ui.Connect(nameof(UI.DeselectRequested), this, nameof(OnDeselectRequested));
            }
        }

        protected virtual void Open()
        {
            EmitSignal(nameof(Opened), this);
        }

        protected virtual void Close()
        {
            EmitSignal(nameof(Closed), this);
        }

        protected virtual void Deselect()
        {

        }

        private void OnCloseRequested()
        {
            Close();
        }

        private void OnDeselectRequested()
        {
            Deselect();
        }
    }
}