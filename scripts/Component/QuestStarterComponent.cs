using GameFeel.Singleton;
using Godot;

namespace GameFeel.Component
{
    [Tool]
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
                GetNodeOrNull<SelectableComponent>(_selectableComponentPath)?.Connect(nameof(SelectableComponent.Selected), this, nameof(OnSelected));
            }
        }

        public void StartQuest()
        {
            QuestTracker.StartQuest(_questId);
        }

        public bool IsQuestAvailable()
        {
            return QuestTracker.IsQuestAvailable(_questId);
        }

        private void OnSelected()
        {
            StartQuest();
        }
    }
}