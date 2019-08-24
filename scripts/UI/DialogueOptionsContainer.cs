using GameFeel.Component.Subcomponent;
using Godot;
using GodotTools.Extension;

namespace GameFeel.UI
{
    public class DialogueOptionsContainer : VBoxContainer
    {
        [Signal]
        public delegate void DialogueOptionSelected(DialogueItem dialogueItem);

        private ResourcePreloader _resourcePreloader;

        public override void _Ready()
        {
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
        }

        public void LoadOptions(Godot.Collections.Array<DialogueItem> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var button = _resourcePreloader.InstanceScene<DialogueOptionButton>();
                button.Text = items[i].Title;
                AddChild(button);
                button.Connect("pressed", this, nameof(OnButtonPressed), new Godot.Collections.Array() { items[i] });

                if (i % 2 == 0)
                {
                    button.OffsetAnimation();
                }
            }
        }

        private void OnButtonPressed(DialogueItem dialogueItem)
        {
            EmitSignal(nameof(DialogueOptionSelected), dialogueItem);
        }
    }
}