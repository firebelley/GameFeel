using System.Collections.Generic;
using System.Linq;
using GameFeel.Component.Subcomponent;
using GameFeel.Singleton;
using GameFeel.UI;
using Godot;
using GodotTools.Extension;

namespace GameFeel.Component
{
    public class DialogueComponent : Position2D
    {
        [Signal]
        public delegate void DialogueOptionsPresented();

        [Export]
        private NodePath _selectableComponentPath;

        public override void _Ready()
        {
            if (_selectableComponentPath != null)
            {
                GetNode<SelectableComponent>(_selectableComponentPath).Connect(nameof(SelectableComponent.Selected), this, nameof(OnSelected));
            }
        }

        public void ConnectDialogueUISignals(DialogueUI dialogueUI)
        {
            this.DisconnectAllSignals(dialogueUI);
            dialogueUI.Connect(nameof(DialogueUI.DialogueOptionSelected), this, nameof(OnDialogueOptionSelected));
        }

        private Godot.Collections.Array<string> GetDialogueOptionTitles()
        {
            var arrayOptions = new Godot.Collections.Array<string>();
            foreach (var child in this.GetChildren<DialogueItem>())
            {
                arrayOptions.Add(child.Title);
            }
            return arrayOptions;
        }

        private void OnSelected()
        {
            GameEventDispatcher.DispatchDialogueStartedEvent(this);
            EmitSignal(nameof(DialogueOptionsPresented), GetDialogueOptionTitles());
        }

        private void OnDialogueOptionSelected(int idx)
        {

        }
    }
}