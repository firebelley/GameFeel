using Godot;

namespace GameFeel.UI
{
    public class UI : CanvasLayer
    {
        // TODO: get paths to various ui
        // TODO: define Opened and Closed signals on toggle able ui elements
        // TODO: define signal here to close all ui panels
        // TODO: on dependent ui's, listen to close signal from here to toggle panel
        // TODO: forward all unhandled events to main
        private Control _rootControl;

        public override void _Ready()
        {
            _rootControl = GetNode<Control>("Control");
            _rootControl.Connect("gui_input", this, nameof(OnGuiInput));
        }

        private void OnGuiInput(InputEvent evt)
        {
            if (evt.IsActionPressed("select"))
            {

            }
        }
    }
}