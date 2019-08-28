using System.Collections.Generic;
using GameFeel.Component.Subcomponent;
using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;

namespace GameFeel.Component
{
    public class DialogueComponent : Position2D
    {
        [Export(PropertyHint.MultilineText)]
        public string Introduction { get; private set; }

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
            foreach (var dialogueItem in this.GetChildren<DialogueItem>())
            {
                if (dialogueItem == null) continue;
                if (dialogueItem.IsValid())
                {
                    arrayOptions.Add(dialogueItem);
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