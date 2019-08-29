using System.Collections.Generic;
using GameFeel.Component.Subcomponent;
using GameFeel.Singleton;
using Godot;

namespace GameFeel.Component
{
    public class DialogueComponent : Position2D
    {
        [Export(PropertyHint.MultilineText)]
        public string Introduction { get; private set; }

        [Export]
        private NodePath _selectableComponentPath;

        private AnimationPlayer _animationPlayer;

        public override void _Ready()
        {
            GetNodeOrNull<SelectableComponent>(_selectableComponentPath ?? string.Empty)?.Connect(nameof(SelectableComponent.Selected), this, nameof(OnSelected));
            _animationPlayer = GetNode<AnimationPlayer>("Sprite/AnimationPlayer");
        }

        public List<DialogueItem> GetValidDialogueItems()
        {
            var arrayOptions = new List<DialogueItem>();
            foreach (var child in GetChildren())
            {
                if (child is DialogueItem dialogueItem && dialogueItem.IsValid())
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