using Godot;
using GodotTools.Extension;

namespace GameFeel.UI
{
    public class DialogueOptionsContainer : VBoxContainer
    {
        [Signal]
        public delegate void DialogueOptionSelected(int idx);

        private ResourcePreloader _resourcePreloader;

        public override void _Ready()
        {
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
        }

        public void LoadOptions(Godot.Collections.Array<string> options)
        {
            for (int i = 0; i < options.Count; i++)
            {
                var button = _resourcePreloader.InstanceScene<DialogueOptionButton>();
                button.Text = options[i];
                AddChild(button);
                button.Connect("pressed", this, nameof(OnButtonPressed), new Godot.Collections.Array() { i });

                if (i % 2 == 0)
                {
                    button.OffsetAnimation();
                }
            }
        }

        private void OnButtonPressed(int idx)
        {
            EmitSignal(nameof(DialogueOptionSelected), idx);
        }
    }
}