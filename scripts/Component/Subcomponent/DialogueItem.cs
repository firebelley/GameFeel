using System.Collections.Generic;
using System.Linq;
using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;

namespace GameFeel.Component.Subcomponent
{
    public class DialogueItem : Node
    {
        [Export]
        public string Title { get; private set; }

        [Export]
        public string ActiveQuestModelId { get; private set; }

        [Export]
        public bool ShowImmediately { get; private set; }

        // comma-separated list of ids
        [Export(PropertyHint.MultilineText)]
        public string RequiredCompletedQuestIds { get; private set; }

        private string[] _requiredCompletedQuestIds;

        public override void _Ready()
        {
            _requiredCompletedQuestIds = !string.IsNullOrEmpty(RequiredCompletedQuestIds) ? RequiredCompletedQuestIds.Split(",", false) : null;
            _requiredCompletedQuestIds = _requiredCompletedQuestIds.Select(x => x.Trim()).ToArray();
        }

        public List<DialogueLine> GetValidLines()
        {
            return this.GetChildren<DialogueLine>();
        }

        public bool IsValid()
        {
            var valid = true;
            if (!string.IsNullOrEmpty(ActiveQuestModelId))
            {
                valid = valid && QuestTracker.GetActiveModel(ActiveQuestModelId) != null;
            }
            if (_requiredCompletedQuestIds != null)
            {
                valid = valid && _requiredCompletedQuestIds.All(x => QuestTracker.IsQuestCompleted(x));
            }
            return valid && !IsQuestActive();
        }

        private bool IsQuestActive()
        {
            var questStarters = GetValidLines().Where(x => x.IsQuestStarter() && !x.IsQuestAvailable());
            return questStarters.Count() > 0;
        }
    }
}