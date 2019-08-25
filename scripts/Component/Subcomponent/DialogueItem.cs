using System.Collections.Generic;
using Godot;
using GodotTools.Extension;

namespace GameFeel.Component.Subcomponent
{
    public class DialogueItem : Node
    {
        [Export]
        public string Title { get; private set; }

        [Export]
        public string ActiveQuestModelId { get; private set; }

        [Export]
        public bool ShowImmediately { get; private set; }

        public List<DialogueLine> GetValidLines()
        {
            return this.GetChildren<DialogueLine>();
        }
    }
}