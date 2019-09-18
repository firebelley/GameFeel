using GameFeel.Data.Model;
using GameFeel.Singleton;
using Godot;

namespace GameFeel.Component.Subcomponent
{
    public class DialogueLine : Node
    {
        [Export(PropertyHint.MultilineText)]
        public string Text { get; private set; }

        // start a quest on dialogue finish
        private QuestStarterComponent _questStarterComponent;

        public override void _Ready()
        {
            if (GetChildCount() > 0)
            {
                _questStarterComponent = GetChildOrNull<QuestStarterComponent>(0);
            }
        }

        public QuestModel GetAssociatedQuestModel()
        {
            var parent = GetParentOrNull<DialogueItem>();
            if (parent == null)
            {
                return null;
            }

            return QuestTracker.GetActiveModel(parent.ActiveQuestModelId);
        }

        public bool IsQuestAvailable()
        {
            if (IsQuestStarter())
            {
                return _questStarterComponent.IsQuestAvailable();
            }
            return false;
        }

        public bool IsQuestStarter()
        {
            return IsInstanceValid(_questStarterComponent);
        }

        public bool IsQuestTurnIn()
        {
            var model = GetAssociatedQuestModel();
            return model is QuestEventModel qem && qem.EventId == GameEventDispatcher.ITEM_TURNED_IN;
        }

        public void StartQuest()
        {
            _questStarterComponent?.StartQuest();
        }
    }
}