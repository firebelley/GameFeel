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
        public string RequiredQuestStageId { get; private set; }

        [Export]
        public bool ShowImmediately { get; private set; }

        // start a quest on dialogue finish
        [Export]
        private NodePath _questStarterComponentPath;

        private QuestStarterComponent _questStarterComponent;

        public override void _Ready()
        {
            if (_questStarterComponentPath != null)
            {
                _questStarterComponent = GetNode<QuestStarterComponent>(_questStarterComponentPath);
            }
        }

        public bool LineStartsQuest(int idx)
        {
            return GetChildCount() - 1 == idx && IsInstanceValid(_questStarterComponent);
        }

        public List<DialogueLine> GetValidLines()
        {
            return this.GetChildren<DialogueLine>();
        }

        private bool CheckCompletion(int childIdx)
        {
            if (GetChildCount() == childIdx)
            {
                _questStarterComponent?.StartQuest();
                return true;
            }
            return false;
        }
    }
}