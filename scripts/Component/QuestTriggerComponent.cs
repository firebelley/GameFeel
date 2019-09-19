using GameFeel.Resource;
using GameFeel.Singleton;
using Godot;

namespace GameFeel.Component
{
    public abstract class QuestTriggerComponent : Node2D
    {
        [Export]
        private string _questModelId;

        public override void _Ready()
        {
            Setup();
        }

        protected abstract void Trigger();

        private void Setup()
        {
            QuestTracker.Instance.Connect(nameof(QuestTracker.PreQuestStarted), this, nameof(OnPreQuestStarted));
            var activeQuest = QuestTracker.GetActiveQuestContainingModelId(_questModelId);
            if (activeQuest != null)
            {
                ConnectQuest(activeQuest);
            }
            InitialCheck();
        }

        private void ConnectQuest(Quest quest)
        {
            if (quest.ContainsModelId(_questModelId))
            {
                quest.Connect(nameof(Quest.QuestModelActivated), this, nameof(OnQuestModelActivated));
            }
        }

        private void InitialCheck()
        {
            if (QuestTracker.GetActiveModel(_questModelId) != null)
            {
                Trigger();
            }
        }

        private void OnPreQuestStarted(Quest quest)
        {
            ConnectQuest(quest);
        }

        private void OnQuestModelActivated(Quest quest, string modelId)
        {
            if (_questModelId == modelId)
            {
                Trigger();
            }
        }
    }
}