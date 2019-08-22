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

        private ResourcePreloader _resourcePreloader;
        private Control _dialogueWindow;
        private Control _rootControl;
        private Control _dialogueContent;
        private DialogueComponent _activeDialogueComponent;
        private DialogueItem _activeDialogueItem;

        public override void _Ready()
        {
            this.SetNodesByDeclaredNodePaths();
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
            _dialogueWindow.Hide();
            GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventDialogueStarted), this, nameof(OnDialogueStarted));
            _rootControl.Connect("gui_input", this, nameof(OnGuiInput));
            _rootControl.Connect("visibility_changed", this, nameof(OnRootControlVisibilityChanged));
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
            _dialogueWindow.RectSize = Vector2.Zero;
        }

        private void OnRootControlVisibilityChanged()
        {
            if (!_rootControl.Visible)
            {
                ClearContainer();
            }
        }

        private void OnDialogueStarted(string eventGuid, DialogueComponent dialogueComponent)
        {
            ClearContainer();
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

        private void OnDialogueOptionSelected(int buttonIdx)
        {
            EmitSignal(nameof(DialogueOptionSelected), buttonIdx);
        }

        private void OnDialogueOptionsPresented(Godot.Collections.Array<string> options)
        {
            var container = _resourcePreloader.InstanceScene<DialogueOptionsContainer>();
            _dialogueContent.AddChild(container);
            container.LoadOptions(options);
            container.Connect(nameof(DialogueOptionsContainer.DialogueOptionSelected), this, nameof(OnDialogueOptionSelected));
        }

        private void OnDialogueItemPresented(DialogueItem dialogueItem)
        {
            if (IsInstanceValid(_activeDialogueItem))
            {
                _activeDialogueItem.DisconnectAllSignals(this);
            }

            _activeDialogueItem = dialogueItem;
            ClearContainer();

            this.DisconnectAllSignals(dialogueItem);
            dialogueItem.Connect(nameof(DialogueItem.LinePresented), this, nameof(OnDialogueLinePresented));
            dialogueItem.Connect(nameof(DialogueItem.LinesFinished), this, nameof(OnDialogueLinesFinished));
            dialogueItem.ConnectDialogueUISignals(this);
            dialogueItem.StartLines();
        }

        private void OnDialogueLinePresented(DialogueItem dialogueItem, DialogueLine dialogueLine)
        {
            ClearContainer();
            var container = _resourcePreloader.InstanceScene<DialogueLineContainer>();
            _dialogueContent.AddChild(container);
            container.DisplayLine(dialogueLine.Text);

            if (dialogueItem.LineStartsQuest(dialogueLine.GetIndex()))
            {
                container.ShowQuestAcceptanceButtons();
            }

            container.Connect(nameof(DialogueLineContainer.NextButtonPressed), this, nameof(OnNextLineButtonPressed), new Godot.Collections.Array() { dialogueLine.GetIndex() + 1 });
            container.Connect(nameof(DialogueLineContainer.QuestAcceptanceIndicated), this, nameof(OnQuestAcceptanceIndicated), new Godot.Collections.Array() { dialogueLine.GetIndex() + 1 });
        }

        private void OnNextLineButtonPressed(int nextIdx)
        {
            EmitSignal(nameof(LineAdvanceRequested), nextIdx);
        }

        private void OnQuestAcceptanceIndicated(bool accepted, int nextIdx)
        {
            if (accepted)
            {
                EmitSignal(nameof(LineAdvanceRequested), nextIdx);
            }
            else
            {
                _rootControl.Hide();
            }
        }

        private void OnDialogueLinesFinished()
        {
            _rootControl.Hide();
        }
    }
}