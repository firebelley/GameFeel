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
        public delegate void DialogueOptionsPresented(Godot.Collections.Array<DialogueItem> dialogueItems);
        [Signal]
        public delegate void DialogueItemPresented(DialogueItem dialogueItem);

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

        private Godot.Collections.Array<DialogueItem> GetValidDialogueItems()
        {
            var arrayOptions = new Godot.Collections.Array<DialogueItem>();
            foreach (var child in this.GetChildren<DialogueItem>())
            {
                var valid = true;
                if (!string.IsNullOrEmpty(child.RequiredQuestStageId))
                {
                    valid = QuestTracker.IsStageActive(child.RequiredQuestStageId);
                }

                if (valid)
                {
                    arrayOptions.Add(child);
                }
            }
            return arrayOptions;
        }

        private void OnSelected()
        {
            GameEventDispatcher.DispatchDialogueStartedEvent(this);
            EmitSignal(nameof(DialogueOptionsPresented), GetValidDialogueItems());
        }

        private void OnDialogueOptionSelected(DialogueItem dialogueItem)
        {
            var item = this.GetChildren<DialogueItem>().FirstOrDefault(x => x == dialogueItem);
            if (IsInstanceValid(item))
            {
                EmitSignal(nameof(DialogueItemPresented), item);
            }
        }
    }
}