using System.Collections.Generic;
using System.Linq;
using GameFeel.Component;
using GameFeel.Component.Subcomponent;
using GameFeel.Data.Model;
using GameFeel.Resource;
using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;

namespace GameFeel.UI
{
    public class DialogueUI : ToggleUI
    {
        private const string INPUT_SELECT = "select";

        [Export]
        private NodePath _dialogueWindowPath;
        [Export]
        private NodePath _dialogueContentPath;

        private Queue<DialogueItem> _itemsToDisplay = new Queue<DialogueItem>();
        private Queue<DialogueLine> _linesToDisplay = new Queue<DialogueLine>();
        private ResourcePreloader _resourcePreloader;
        private Control _dialogueWindow;
        private Control _dialogueContent;
        private DialogueComponent _activeDialogueComponent;

        public override void _Ready()
        {
            base._Ready();
            this.SetNodesByDeclaredNodePaths();
            Close();
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
            GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventDialogueStarted), this, nameof(OnDialogueStarted));
        }

        public override void _Process(float delta)
        {
            if (IsInstanceValid(_activeDialogueComponent))
            {
                UpdateBubblePosition();
            }
            else
            {
                Close();
            }
        }

        protected override void Open()
        {
            base.Open();
            Show();
            ClearAll();
            SetProcess(true);
        }

        protected override void Close()
        {
            base.Close();
            Hide();
            ClearAll();
            SetProcess(false);
        }

        private void UpdateBubblePosition()
        {
            var pos = _activeDialogueComponent.GetGlobalTransformWithCanvas().origin / Main.UI_TO_GAME_DISPLAY_RATIO;
            _dialogueWindow.RectPosition = pos - new Vector2(_dialogueWindow.RectSize.x / 2f, _dialogueWindow.RectSize.y);
        }

        private void ClearContainer()
        {
            foreach (var child in _dialogueContent.GetChildren())
            {
                if (child is Node n)
                {
                    n.GetParent().RemoveChild(n);
                    n.QueueFree();
                }
            }
            _dialogueWindow.RectSize = Vector2.Zero;
        }

        private void ClearAll()
        {
            ClearContainer();
            _activeDialogueComponent = null;
            _itemsToDisplay.Clear();
            _linesToDisplay.Clear();
        }

        private void OnDialogueStarted(string eventGuid, DialogueComponent dialogueComponent)
        {
            Open();
            _activeDialogueComponent = dialogueComponent;
            InitializeContent();
        }

        private void InitializeContent()
        {
            var validItems = _activeDialogueComponent.GetValidDialogueItems();
            foreach (var item in validItems.Where(x => x.ShowImmediately))
            {
                _itemsToDisplay.Enqueue(item);
            }
            AdvanceItem();
            UpdateBubblePosition();
        }

        private void ShowNavigation()
        {
            ClearContainer();
            var container = _resourcePreloader.InstanceScene<DialogueOptionsContainer>();
            _dialogueContent.AddChild(container);
            container.LoadOptions(_activeDialogueComponent.GetValidDialogueItems());
            container.SetIntroductionText(_activeDialogueComponent.Introduction);
            container.Connect(nameof(DialogueOptionsContainer.DialogueOptionSelected), this, nameof(OnDialogueOptionSelected));
        }

        private void AdvanceItem()
        {
            if (_itemsToDisplay.Count > 0)
            {
                var item = _itemsToDisplay.Dequeue();
                foreach (var line in item.GetValidLines())
                {
                    _linesToDisplay.Enqueue(line);
                }
                AdvanceLine();
            }
            else
            {
                ShowNavigation();
            }
        }

        private void AdvanceLine()
        {
            if (_linesToDisplay.Count > 0)
            {
                ClearContainer();
                var line = _linesToDisplay.Dequeue();
                var container = _resourcePreloader.InstanceScene<DialogueLineContainer>();
                _dialogueContent.AddChild(container);
                container.DisplayLine(line);
                container.Connect(nameof(DialogueLineContainer.NextButtonPressed), this, nameof(OnNextLineButtonPressed));
                container.Connect(nameof(DialogueLineContainer.QuestAcceptanceIndicated), this, nameof(OnQuestAcceptanceIndicated), new Godot.Collections.Array() { line });
                container.Connect(nameof(DialogueLineContainer.QuestTurnInIndicated), this, nameof(OnQuestTurnInIndicated), new Godot.Collections.Array() { line });
            }
            else
            {
                AdvanceItem();
            }
        }

        private void OnDialogueOptionSelected(DialogueItem dialogueItem)
        {
            _itemsToDisplay.Enqueue(dialogueItem);
            AdvanceItem();
        }

        private void OnNextLineButtonPressed()
        {
            AdvanceLine();
        }

        private void OnQuestAcceptanceIndicated(bool accepted, DialogueLine dialogueLine)
        {
            if (accepted)
            {
                dialogueLine.StartQuest();
                AdvanceLine();
            }
            else
            {
                AdvanceItem();
            }
        }

        private void OnQuestTurnInIndicated(DialogueLine dialogueLine)
        {
            var model = dialogueLine.GetAssociatedQuestModel();
            if (Quest.IsQuestEventReadyForCompletion(model))
            {
                var evt = model as QuestEventModel;
                GameEventDispatcher.DispatchItemTurnedInEvent(evt.Id, evt.ItemId, evt.Required);
                AdvanceLine();
            }
        }
    }
}