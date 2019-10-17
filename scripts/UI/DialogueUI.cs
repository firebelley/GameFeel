using System.Collections.Generic;
using System.Linq;
using GameFeel.Component;
using GameFeel.Component.Subcomponent;
using GameFeel.Data.Model;
using GameFeel.Resource;
using GameFeel.Singleton;
using Godot;
using GodotApiTools.Extension;

namespace GameFeel.UI
{
    public class DialogueUI : ToggleUI
    {
        private const string INPUT_SELECT = "select";
        private const string ANIM_BOUNCE_IN = "ControlBounceIn";
        private const string ANIM_BOUNCE_IN_SECONDARY = "ControlBounceInSecondary";

        [Export]
        private NodePath _dialogueWindowPath;
        [Export]
        private NodePath _dialogueContentPath;
        [Export]
        private NodePath _animationPlayerPath;
        [Export]
        private NodePath _nameplatePath;

        private Queue<DialogueItem> _itemsToDisplay = new Queue<DialogueItem>();
        private Queue<DialogueLine> _linesToDisplay = new Queue<DialogueLine>();
        private ResourcePreloader _resourcePreloader;
        private Control _dialogueWindow;
        private Control _dialogueContent;
        private DialogueComponent _activeDialogueComponent;
        private AnimationPlayer _animationPlayer;
        private Label _nameplate;

        private bool _closeAfterLinesShown = false;

        public override void _Ready()
        {
            base._Ready();
            this.SetNodesByDeclaredNodePaths();
            CloseImmediate();
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
            GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventDialogueStarted), this, nameof(OnDialogueStarted));
            _animationPlayer.Connect("animation_finished", this, nameof(OnAnimationFinished));
            _dialogueWindow.Connect("mouse_entered", this, nameof(OnWindowMouseEntered));
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
            _animationPlayer.PlaybackSpeed = 1f;
            _animationPlayer.Play(ANIM_BOUNCE_IN);
        }

        protected override void Close()
        {
            base.Close();
            _animationPlayer.PlaybackSpeed = 2f;
            _animationPlayer.PlayBackwards(ANIM_BOUNCE_IN_SECONDARY);
        }

        private void CloseImmediate()
        {
            ClearAll();
            Hide();
            SetProcess(false);
            _dialogueWindow.RectScale = Vector2.Zero;
        }

        private void UpdateBubblePosition()
        {
            _dialogueWindow.RectPivotOffset = _dialogueWindow.RectSize / 2f;
            var pos = _activeDialogueComponent.GetGlobalTransformWithCanvas().origin / Main.UI_TO_GAME_DISPLAY_RATIO;
            _dialogueWindow.RectPosition = pos - new Vector2(_dialogueWindow.RectSize.x / 2f, _dialogueWindow.RectSize.y);
        }

        private void UpdateEntityData(EntityDataComponent entityData)
        {
            if (entityData == null)
            {
                _nameplate.Hide();
                return;
            }

            _nameplate.Show();
            _nameplate.Text = entityData.DisplayName;
        }

        private void ClearContainer()
        {
            foreach (var child in _dialogueContent.GetChildren())
            {
                if (child is Container n)
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
            _closeAfterLinesShown = false;
            _itemsToDisplay.Clear();
            _linesToDisplay.Clear();
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
            UpdateEntityData(_activeDialogueComponent.GetEntityDataComponent());
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
                _linesToDisplay.Clear();
                foreach (var line in item.GetValidLines())
                {
                    _linesToDisplay.Enqueue(line);
                }
                AdvanceLine();
            }
            else if (_closeAfterLinesShown)
            {
                Close();
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

                if (_linesToDisplay.Count == 0)
                {
                    container.SetupLastLine();
                }

                container.DisplayLine(line);
                container.Connect(nameof(DialogueLineContainer.NextButtonPressed), this, nameof(OnNextLineButtonPressed), new Godot.Collections.Array() { line });
                container.Connect(nameof(DialogueLineContainer.QuestAcceptanceIndicated), this, nameof(OnQuestAcceptanceIndicated), new Godot.Collections.Array() { line });
                container.Connect(nameof(DialogueLineContainer.QuestTurnInIndicated), this, nameof(OnQuestTurnInIndicated), new Godot.Collections.Array() { line });
            }
            else
            {
                AdvanceItem();
            }
        }

        private void ShowTurnIn(DialogueLine dialogueLine)
        {
            ClearContainer();
            var container = _resourcePreloader.InstanceScene<DialogueLineContainer>();
            _dialogueContent.AddChild(container);
            container.DisplayTurnIn((QuestEventModel) dialogueLine.GetAssociatedQuestModel());
            container.Connect(nameof(DialogueLineContainer.QuestTurnInIndicated), this, nameof(OnQuestTurnInIndicated), new Godot.Collections.Array() { dialogueLine });
            container.Connect(nameof(DialogueLineContainer.NotYetButtonPressed), this, nameof(OnNotYetButtonPressed));
        }

        private void OnDialogueStarted(string eventGuid, DialogueComponent dialogueComponent)
        {
            Open();
            _activeDialogueComponent = dialogueComponent;
            InitializeContent();
        }

        private void OnDialogueOptionSelected(DialogueItem dialogueItem)
        {
            _itemsToDisplay.Enqueue(dialogueItem);
            AdvanceItem();
        }

        private void OnNextLineButtonPressed(DialogueLine dialogueLine)
        {
            if (dialogueLine.IsQuestTurnIn() && _linesToDisplay.Count == 0)
            {
                ShowTurnIn(dialogueLine);
            }
            else
            {
                AdvanceLine();
            }
        }

        private void OnQuestAcceptanceIndicated(bool accepted, DialogueLine dialogueLine)
        {
            if (accepted)
            {
                dialogueLine.StartQuest();
                _closeAfterLinesShown = true;
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
                _closeAfterLinesShown = true;
                AdvanceLine();
            }
        }

        private void OnAnimationFinished(string anim)
        {
            if (anim == ANIM_BOUNCE_IN_SECONDARY)
            {
                CloseImmediate();
            }
        }

        private void OnNotYetButtonPressed()
        {
            AdvanceItem();
        }

        private void OnWindowMouseEntered()
        {
            var evt = new InputEventMouseMotion();
            evt.Position = Vector2.Left * 100_000_000;
            Main.SendInput(evt);
        }
    }
}