using GameFeel.Data.Model;
using GameFeel.Resource;
using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;

namespace GameFeel.UI
{
    public class QuestTrackerUI : VBoxContainer
    {
        private ResourcePreloader _resourcePreloader;

        public override void _Ready()
        {
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
            QuestTracker.Instance.Connect(nameof(QuestTracker.PreQuestStarted), this, nameof(OnNewQuestStarted));
        }

        private QuestTrackItem AttachQuest(Quest quest, string modelId)
        {
            var item = _resourcePreloader.InstanceScene<QuestTrackItem>();
            AddChild(item);
            item.SetQuest(quest, modelId);
            return item;
        }

        private void OnQuestStarted(Quest quest, string modelId)
        {
            AttachQuest(quest, modelId);
        }

        private void OnQuestStageStarted(Quest quest, string modelId)
        {

        }

        private void OnQuestEventCompleted(Quest quest, string modelId)
        {

        }

        private void OnNewQuestStarted(Quest quest)
        {
            quest.Connect(nameof(Quest.QuestStarted), this, nameof(OnQuestStarted));
        }
    }
}