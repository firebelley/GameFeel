using GameFeel.Component;
using GameFeel.Component.Subcomponent;
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
        [Signal]
        public delegate void LineAdvanceRequested(int idx);

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
        private DialogueItem _activeDialogueItem;

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
            dialogueComponent.Connect(nameof(DialogueComponent.DialogueItemPresented), this, nameof(OnDialogueItemPresented));
        }

        private void ClearContainer()
        {
            foreach (var child in _dialogueContent.GetChildren())
            {
                if (child is Node n)
                {
                    n.GetParent().RemoveChild(n);
                    n.QueueFree();
                }
            }
        }

        private void OnDialogueStarted(string eventGuid, DialogueComponent dialogueComponent)
        {
            _activeDialogueComponent = dialogueComponent;
            ConnectDialogueSignals(dialogueComponent);
            dialogueComponent.ConnectDialogueUISignals(this);
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

        private void OnDialogueItemPresented(DialogueItem dialogueItem)
        {
            _activeDialogueItem = dialogueItem;
            ClearContainer();

            this.DisconnectAllSignals(dialogueItem);
            dialogueItem.Connect(nameof(DialogueItem.LinePresented), this, nameof(OnDialogueLinePresented));
            dialogueItem.ConnectDialogueUISignals(this);
            dialogueItem.StartLines();
        }

        private void OnDialogueLinePresented(string line, int lineIdx)
        {
            ClearContainer();
            var label = new Label();
            label.Text = line;
            _dialogueContent.AddChild(label);

            var button = new Button();
            button.Text = "Next";
            _dialogueContent.AddChild(button);
            button.Connect("pressed", this, nameof(OnNextLineButtonPressed), new Godot.Collections.Array() { lineIdx + 1 });
        }

        private void OnNextLineButtonPressed(int nextIdx)
        {
            EmitSignal(nameof(LineAdvanceRequested), nextIdx);
        }
    }
}