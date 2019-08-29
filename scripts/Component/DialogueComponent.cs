using System.Collections.Generic;
using System.Linq;
using GameFeel.Component.Subcomponent;
using GameFeel.Resource;
using GameFeel.Singleton;
using Godot;

namespace GameFeel.Component
{
    public class DialogueComponent : Position2D
    {
        [Export(PropertyHint.MultilineText)]
        public string Introduction { get; private set; }

        [Export]
        private NodePath _selectableComponentPath;
        [Export]
        private NodePath _entityDataComponentPath;

        private AnimationPlayer _animationPlayer;
        private Sprite _sprite;
        private EntityDataComponent _entityDataComponent;

        public override void _Ready()
        {
            GetNodeOrNull<SelectableComponent>(_selectableComponentPath ?? string.Empty)?.Connect(nameof(SelectableComponent.Selected), this, nameof(OnSelected));
            _entityDataComponent = GetNodeOrNull<EntityDataComponent>(_entityDataComponentPath ?? string.Empty);
            _animationPlayer = GetNode<AnimationPlayer>("Sprite/AnimationPlayer");
            _sprite = GetNode<Sprite>("Sprite");
            UpdateQuestIndicators();

            QuestTracker.Instance.Connect(nameof(QuestTracker.PreQuestStarted), this, nameof(OnQuestStarted));
        }

        public List<DialogueItem> GetValidDialogueItems()
        {
            var arrayOptions = new List<DialogueItem>();
            foreach (var child in GetChildren())
            {
                if (child is DialogueItem dialogueItem && dialogueItem.IsValid())
                {
                    arrayOptions.Add(dialogueItem);
                }
            }
            return arrayOptions;
        }

        public EntityDataComponent GetEntityDataComponent()
        {
            return _entityDataComponent;
        }

        private void UpdateQuestIndicators()
        {
            var questIndicatorShown = false;
            foreach (var child in GetChildren())
            {
                if (child is DialogueItem dialogueItem)
                {
                    if (dialogueItem.IsValid())
                    {
                        if (IsReadyForCompletion(dialogueItem))
                        {
                            ShowQuestTurnIn();
                            questIndicatorShown = true;
                            break;
                        }
                        if (dialogueItem.HasAvailableQuest())
                        {
                            ShowQuestAvailable();
                            questIndicatorShown = true;
                        }
                    }
                }
            }

            if (!questIndicatorShown)
            {
                HideQuestIndicator();
            }
        }

        private bool IsReadyForCompletion(DialogueItem dialogueItem)
        {
            return dialogueItem
                .GetValidLines()
                .Select(x => x.GetAssociatedQuestModel())
                .Where(x => x != null && Quest.IsQuestEventReadyForCompletion(x))
                .Count() > 0;
        }

        private void ShowQuestAvailable()
        {
            _sprite.Show();
            _sprite.Frame = 0;
            _animationPlayer.Play();
        }

        private void ShowQuestTurnIn()
        {
            _sprite.Show();
            _sprite.Frame = 1;
            _animationPlayer.Play();
        }

        private void HideQuestIndicator()
        {
            _sprite.Hide();
            _animationPlayer.Stop();
        }

        private void OnSelected()
        {
            GameEventDispatcher.DispatchDialogueStartedEvent(this);
        }

        private void OnQuestStarted(Quest quest)
        {
            quest.Connect(nameof(Quest.QuestCompleted), this, nameof(OnQuestUpdate));
            quest.Connect(nameof(Quest.QuestEventCompleted), this, nameof(OnQuestUpdate));
            quest.Connect(nameof(Quest.QuestEventStarted), this, nameof(OnQuestUpdate));
            quest.Connect(nameof(Quest.QuestStageStarted), this, nameof(OnQuestUpdate));
            quest.Connect(nameof(Quest.QuestStarted), this, nameof(OnQuestUpdate));
        }

        private void OnQuestUpdate(Quest quest, string modelId)
        {
            UpdateQuestIndicators();
        }
    }
}