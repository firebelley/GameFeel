using Godot;

namespace GameFeel.Component.Subcomponent
{
    public class DialogueItem : Node
    {
        [Export]
        // start a quest on dialogue finish
        public NodePath QuestStarterComponentPath { get; private set; }

        [Export]
        public string Title { get; private set; }

        public override void _Ready()
        {

        }
    }
}