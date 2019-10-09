using GameFeel.Singleton;
using Godot;

namespace GameFeel.Component
{
    [Tool]
    public class QuestStarterComponent : Node
    {
        [Export(PropertyHint.File, "*.quest")]
        private string _questFile;
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
            QuestTracker.StartQuest(_questFile);
        }

        public bool IsQuestAvailable()
        {
            return QuestTracker.IsQuestAvailable(_questFile);
        }

        private void OnSelected()
        {
            StartQuest();
        }
    }
}