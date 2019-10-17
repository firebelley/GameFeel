using System.Collections.Generic;
using System.Linq;
using GameFeel.Singleton;
using Godot;
using GodotApiTools.Extension;

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
        public string[] RequiredCompletedQuestIds { get; private set; }

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
            if (RequiredCompletedQuestIds != null)
            {
                valid = valid && RequiredCompletedQuestIds.All(x => QuestTracker.IsQuestCompleted(x));
            }
            if (HasQuestStarter())
            {
                valid = valid && HasAvailableQuest();
            }
            return valid;
        }

        public bool HasAvailableQuest()
        {
            return GetValidLines().Any(x => x.IsQuestAvailable());
        }

        public bool HasQuestStarter()
        {
            return GetValidLines().Any(x => x.IsQuestStarter());
        }
    }
}