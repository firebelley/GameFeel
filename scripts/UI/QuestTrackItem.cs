using GameFeel.Data.Model;
using GameFeel.Resource;
using Godot;

namespace GameFeel.UI
{
    public class QuestTrackItem : VBoxContainer
    {
        private const string NUMBER_TRACKER_FORMAT = "{0}/{1} ";

        private Label _questNameLabel;
        private Label _questStageNameLabel;
        private Label _questPromptLabel;

        public override void _Ready()
        {
            _questNameLabel = GetNode<Label>("QuestNameLabel");
            _questStageNameLabel = GetNode<Label>("QuestStageNameLabel");
            _questPromptLabel = GetNode<Label>("QuestPromptLabel");

            _questNameLabel.Visible = false;
            _questPromptLabel.Visible = false;
            _questStageNameLabel.Visible = false;
        }

        public void SetQuest(Quest quest, string modelId)
        {
            quest.Connect(nameof(Quest.QuestStageStarted), this, nameof(OnQuestStageStarted));
            quest.Connect(nameof(Quest.QuestEventStarted), this, nameof(OnQuestEventStarted));

            SetQuestStart(quest.GetQuestModel(modelId) as QuestStartModel);
        }

        public void SetQuestStart(QuestStartModel model)
        {
            _questNameLabel.Text = model.DisplayName;
            _questNameLabel.Visible = !string.IsNullOrEmpty(_questNameLabel.Text);
        }

        public void SetQuestStage(QuestStageModel model)
        {
            _questStageNameLabel.Text = model.DisplayName;
            _questStageNameLabel.Visible = !string.IsNullOrEmpty(_questStageNameLabel.Text);
        }

        public void SetQuestPrompt(QuestEventModel questEventModel)
        {
            var prefix = questEventModel.Required > 0 ? string.Format(NUMBER_TRACKER_FORMAT, 0, questEventModel.Required) : string.Empty;
            _questPromptLabel.Text = prefix + questEventModel.PromptText;
            _questPromptLabel.Visible = !string.IsNullOrEmpty(_questPromptLabel.Text);
        }

        public void AddQuestStage()
        {

        }

        public void AddQuestPrompt()
        {

        }

        private void OnQuestStageStarted(Quest quest, string modelId)
        {
            SetQuestStage(quest.GetQuestModel(modelId) as QuestStageModel);
        }

        private void OnQuestEventStarted(Quest quest, string modelId)
        {
            SetQuestPrompt(quest.GetQuestModel(modelId) as QuestEventModel);
        }
    }
}