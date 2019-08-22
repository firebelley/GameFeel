using Godot;

namespace GameFeel.Component.Subcomponent
{
    public class DialogueLine : Node
    {
        [Export(PropertyHint.MultilineText)]
        public string Text { get; private set; }

        public override void _Ready()
        {

        }
    }
}