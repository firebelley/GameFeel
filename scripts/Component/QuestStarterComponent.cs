using GameFeel.Singleton;
using Godot;

namespace GameFeel.Component
{
    public class QuestStarterComponent : Node
    {
        [Export]
        private string _questId;
        [Export]
        private NodePath _selectableComponentPath;

        public override void _Ready()
        {
            if (_selectableComponentPath != null)
            {
                GetNode<SelectableComponent>(_selectableComponentPath).Connect(nameof(SelectableComponent.Selected), this, nameof(OnSelected));
            }
        }

        public void StartQuest()
        {
            QuestTracker.StartQuest(_questId);
        }

        private void OnSelected()
        {
            StartQuest();
        }
    }
}