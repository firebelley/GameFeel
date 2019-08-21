using GameFeel.Component;
using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;

namespace GameFeel.UI
{
    public class DialogueUI : CanvasLayer
    {
        private const string INPUT_SELECT = "select";

        [Signal]
        public delegate void DialogueOptionSelected(int idx);

        [Export]
        private NodePath _rootControlPath;
        [Export]
        private NodePath _dialogueWindowPath;
        [Export]
        private NodePath _dialogueContentPath;

        private Control _dialogueWindow;
        private Control _rootControl;
        private Control _dialogueContent;
        private DialogueComponent _activeDialogueComponent;

        public override void _Ready()
        {
            this.SetNodesByDeclaredNodePaths();
            _dialogueWindow.Hide();
            GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventDialogueStarted), this, nameof(OnDialogueStarted));
            _rootControl.Connect("gui_input", this, nameof(OnGuiInput));
        }

        public override void _Process(float delta)
        {
            if (IsInstanceValid(_activeDialogueComponent))
            {
                var pos = _activeDialogueComponent.GetGlobalTransformWithCanvas().origin / Main.UI_TO_GAME_DISPLAY_RATIO;
                _dialogueWindow.RectPosition = pos - new Vector2(_dialogueWindow.RectSize.x / 2f, _dialogueWindow.RectSize.y);
            }
            else
            {
                _rootControl.Hide();
            }
        }

        private void ConnectDialogueSignals(DialogueComponent dialogueComponent)
        {
            this.DisconnectAllSignals(dialogueComponent);
            dialogueComponent.Connect(nameof(DialogueComponent.DialogueOptionsPresented), this, nameof(OnDialogueOptionsPresented));
        }

        private void OnDialogueStarted(string eventGuid, DialogueComponent dialogueComponent)
        {
            _activeDialogueComponent = dialogueComponent;
            ConnectDialogueSignals(dialogueComponent);
            _dialogueWindow.Show();
            _rootControl.Show();
        }

        private void OnGuiInput(InputEvent evt)
        {
            if (evt.IsActionPressed(INPUT_SELECT))
            {
                _rootControl.Hide();
                _rootControl.AcceptEvent();
            }
        }

        private void OnDialogueItemButtonPressed(int buttonIdx)
        {
            EmitSignal(nameof(DialogueOptionSelected), buttonIdx);
        }

        private void OnDialogueOptionsPresented(Godot.Collections.Array<string> options)
        {
            for (int i = 0; i < options.Count; i++)
            {
                var button = new Button();
                button.Text = options[i];
                _dialogueContent.AddChild(button);
                button.Connect("pressed", this, nameof(OnDialogueItemButtonPressed), new Godot.Collections.Array() { i });
            }
        }
    }
}