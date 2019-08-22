using Godot;

namespace GameFeel.UI
{
    public class DialogueOptionsContainer : VBoxContainer
    {
        [Signal]
        public delegate void DialogueOptionSelected(int idx);

        public override void _Ready()
        {

        }

        public void LoadOptions(Godot.Collections.Array<string> options)
        {
            for (int i = 0; i < options.Count; i++)
            {
                var button = new Button();
                button.Text = options[i];
                AddChild(button);
                button.Connect("pressed", this, nameof(OnButtonPressed), new Godot.Collections.Array() { i });
            }
        }

        private void OnButtonPressed(int idx)
        {
            EmitSignal(nameof(DialogueOptionSelected), idx);
        }
    }
}