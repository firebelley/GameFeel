using GameFeel.Resource;
using GameFeel.Singleton;
using Godot;

namespace GameFeel.Component
{
    [Tool]
    public class QuestStarterComponent : Node
    {
        [Export]
        private QuestResource _questResource;
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
            QuestTracker.StartQuest(_questResource.QuestId);
        }

        public bool IsQuestAvailable()
        {
            return QuestTracker.IsQuestAvailable(_questResource.QuestId);
        }

        private void OnSelected()
        {
            StartQuest();
        }
    }
}