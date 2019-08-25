using Godot;

namespace GameFeel.Component.Subcomponent
{
    [Tool]
    public class DialogueLine : Node
    {
        [Export(PropertyHint.MultilineText)]
        public string Text { get; private set; }

        [Export]
        public LineType LineContainerType { get; private set; } = LineType.NORMAL;

        // start a quest on dialogue finish
        private QuestStarterComponent _questStarterComponent;

        public enum LineType
        {
            NORMAL,
            TURN_IN,
            QUEST_ACCEPTANCE
        }

        public override void _Ready()
        {
            if (GetChildCount() > 0)
            {
                _questStarterComponent = GetChildOrNull<QuestStarterComponent>(0);
            }
        }

        public override string _GetConfigurationWarning()
        {
            if (!IsInstanceValid(_questStarterComponent) && LineContainerType == LineType.QUEST_ACCEPTANCE)
            {
                return "Must have a quest starter component if Quest Acceptance type";
            }
            return string.Empty;
        }

        public bool IsQuestStarter()
        {
            return IsInstanceValid(_questStarterComponent);
        }

        public bool IsQuestAvailable()
        {
            if (IsInstanceValid(_questStarterComponent))
            {
                return _questStarterComponent.IsQuestAvailable();
            }
            return false;
        }

        public void StartQuest()
        {
            _questStarterComponent?.StartQuest();
        }
    }
}