using System.Collections.Generic;
using System.Linq;
using GameFeel.Component.Subcomponent;
using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;

namespace GameFeel.Component
{
    public class DialogueComponent : Position2D
    {
        [Export]
        private NodePath _selectableComponentPath;

        public override void _Ready()
        {
            if (_selectableComponentPath != null)
            {
                GetNode<SelectableComponent>(_selectableComponentPath).Connect(nameof(SelectableComponent.Selected), this, nameof(OnSelected));
            }
        }

        public List<DialogueItem> GetValidDialogueItems()
        {
            var arrayOptions = new List<DialogueItem>();
            foreach (var di in this.GetChildren<DialogueItem>())
            {
                var valid = true;
                if (!string.IsNullOrEmpty(di.RequiredQuestStageId))
                {
                    valid = QuestTracker.IsStageActive(di.RequiredQuestStageId);
                }

                valid = valid && !HasActiveQuest(di);

                if (valid)
                {
                    arrayOptions.Add(di);
                }
            }
            return arrayOptions;
        }

        private bool HasActiveQuest(DialogueItem dialogueItem)
        {
            var questStarters = dialogueItem.GetValidLines().Where(x => x.IsQuestStarter() && !x.IsQuestAvailable());
            return questStarters.Count() > 0;
        }

        private void OnSelected()
        {
            GameEventDispatcher.DispatchDialogueStartedEvent(this);
        }
    }
}