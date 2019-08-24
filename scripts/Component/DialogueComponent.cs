using System.Collections.Generic;
using GameFeel.Component.Subcomponent;
using GameFeel.Singleton;
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

        public List<DialogueItem> GetValidDialogueItems()
        {
            var arrayOptions = new List<DialogueItem>();
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
        }
    }
}