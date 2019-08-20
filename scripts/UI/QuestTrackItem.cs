using System.Collections.Generic;
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
        private ResourcePreloader _resourcePreloader;

        private Dictionary<string, QuestTrackItem> _eventTrackItemMap = new Dictionary<string, QuestTrackItem>();

        public override void _Ready()
        {
            _questNameLabel = GetNode<Label>("QuestNameLabel");
            _questStageNameLabel = GetNode<Label>("QuestStageNameLabel");
            _questPromptLabel = GetNode<Label>("QuestPromptLabel");
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");

            _questNameLabel.Visible = false;
            _questPromptLabel.Visible = false;
            _questStageNameLabel.Visible = false;
        }

        public void SetQuest(Quest quest, string modelId)
        {
            quest.Connect(nameof(Quest.QuestStageStarted), this, nameof(OnQuestStageStarted));
            quest.Connect(nameof(Quest.QuestEventStarted), this, nameof(OnQuestEventStarted));
            quest.Connect(nameof(Quest.QuestEventProgress), this, nameof(OnQuestEventProgress));
            quest.Connect(nameof(Quest.QuestCompleted), this, nameof(OnQuestCompleted));

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

        public void SetQuestPrompt(Quest quest, QuestEventModel questEventModel)
        {
            var progress = quest.GetEventProgress(questEventModel.Id);
            var prefix = questEventModel.Required > 0 ? string.Format(NUMBER_TRACKER_FORMAT, progress, questEventModel.Required) : string.Empty;
            _questPromptLabel.Text = prefix + questEventModel.PromptText;
            _questPromptLabel.Visible = !string.IsNullOrEmpty(_questPromptLabel.Text);
        }

        public void AddQuestStage(QuestStageModel questStageModel)
        {
            var qti = GD.Load(Filename) as PackedScene;
            var qtis = qti.Instance() as QuestTrackItem;
            AddChild(qtis);
            qtis.SetQuestStage(questStageModel);
        }

        public void AddQuestPrompt(Quest quest, QuestEventModel questEventModel)
        {
            var qti = GD.Load(Filename) as PackedScene;
            var qtis = qti.Instance() as QuestTrackItem;
            AddChild(qtis);
            qtis.SetQuestPrompt(quest, questEventModel);
            _eventTrackItemMap[questEventModel.Id] = qtis;
        }

        private void OnQuestStageStarted(Quest quest, string modelId)
        {
            AddQuestStage(quest.GetQuestModel(modelId) as QuestStageModel);
        }

        private void OnQuestEventStarted(Quest quest, string modelId)
        {
            AddQuestPrompt(quest, quest.GetQuestModel(modelId) as QuestEventModel);
        }

        private void OnQuestEventProgress(Quest quest, string modelId)
        {
            if (_eventTrackItemMap.ContainsKey(modelId))
            {
                var model = quest.GetQuestModel(modelId) as QuestEventModel;
                _eventTrackItemMap[modelId].SetQuestPrompt(quest, model);
            }
        }

        private void OnQuestCompleted(Quest quest, string modelId)
        {
            QueueFree();
        }
    }
}