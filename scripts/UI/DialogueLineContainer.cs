using GameFeel.Component.Subcomponent;
using GameFeel.Data;
using GameFeel.Data.Model;
using GameFeel.Resource;
using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;
using GodotTools.Util;

namespace GameFeel.UI
{
    public class DialogueLineContainer : VBoxContainer
    {
        [Signal]
        public delegate void NextButtonPressed();
        [Signal]
        public delegate void QuestAcceptanceIndicated(bool accepted);
        [Signal]
        public delegate void QuestTurnInIndicated();
        [Signal]
        public delegate void NotYetButtonPressed();

        [Export]
        private NodePath _resourcePreloaderPath;
        [Export]
        private NodePath _dialogueLabelPath;
        [Export]
        private NodePath _acceptButtonPath;
        [Export]
        private NodePath _declineButtonPath;
        [Export]
        private NodePath _nextButtonPath;
        [Export]
        private NodePath _completeButtonPath;
        [Export]
        private NodePath _notYetButtonPath;
        [Export]
        private NodePath _inventoryCellPath;
        [Export]
        private NodePath _rewardsContainerPath;
        [Export]
        private NodePath _requirementsContainerPath;
        [Export]
        private NodePath _turnInContainerPath;

        private Label _dialogueLabel;
        private Button _acceptButton;
        private Button _declineButton;
        private Button _nextButton;
        private Button _completeButton;
        private Button _notYetButton;
        private InventoryCell _inventoryCell;
        private ResourcePreloader _resourcePreloader;
        private Container _rewardsContainer;
        private Container _requirementsContainer;
        private Container _turnInContainer;

        public override void _Ready()
        {
            this.SetNodesByDeclaredNodePaths();
            HideButtons();
            _nextButton.Connect("pressed", this, nameof(OnNextButtonPressed));
            _acceptButton.Connect("pressed", this, nameof(OnAcceptButtonPressed));
            _declineButton.Connect("pressed", this, nameof(OnDeclineButtonPressed));
            _completeButton.Connect("pressed", this, nameof(OnTurnInButtonPressed));
            _notYetButton.Connect("pressed", this, nameof(OnNotYetButtonPressed));
        }

        public void DisplayLine(DialogueLine line)
        {
            HideButtons();
            _dialogueLabel.Text = line.Text;
            _dialogueLabel.Show();

            if (line.IsQuestStarter())
            {
                ShowQuestAcceptanceButtons();
            }
            else
            {
                ShowNextButton();
            }
        }

        public void DisplayTurnIn(QuestEventModel questEventModel)
        {
            HideButtons();
            _dialogueLabel.Hide();
            _turnInContainer.Show();
            _notYetButton.Show();

            ShowTurnInRequirements(questEventModel);

            if (Quest.IsQuestEventReadyForCompletion(questEventModel))
            {
                _completeButton.Show();
            }
        }

        public void SetupLastLine()
        {
            _nextButton.Text = "Continue";
        }

        private void ShowNextButton()
        {
            _nextButton.Show();
        }

        private void ShowQuestAcceptanceButtons()
        {
            HideButtons();
            _acceptButton.Show();
            _declineButton.Show();
        }

        private void HideButtons()
        {
            _acceptButton.Hide();
            _declineButton.Hide();
            _nextButton.Hide();
            _completeButton.Hide();
            _notYetButton.Hide();
            _turnInContainer.Hide();
        }

        private void SetupInventoryCell(DialogueLine dialogueLine)
        {
            var model = dialogueLine.GetAssociatedQuestModel();
            if (model == null)
            {
                Logger.Error("Could not find associated quest model");
                return;
            }
            if (model is QuestEventModel qem)
            {
                var metadata = MetadataLoader.LootItemIdToMetadata[qem.ItemId];
                var item = InventoryItem.FromMetadata(metadata);
                item.Amount = qem.Required;
                _inventoryCell.SetInventoryItem(item);
            }
            else
            {
                Logger.Error("Could not parse model as an event model " + model.Id);
            }
        }

        private void ShowTurnInRequirements(QuestEventModel questEventModel)
        {
            var quest = QuestTracker.GetActiveQuestContainingModelId(questEventModel.Id);
            var rewards = quest.GetRewards(questEventModel.Id);

            foreach (var child in _rewardsContainer.GetChildren<Node>())
            {
                if (child is InventoryCell)
                {
                    child.GetParent().RemoveChild(child);
                    child.QueueFree();
                }
            }

            foreach (var child in _requirementsContainer.GetChildren<Node>())
            {
                if (child is InventoryCell)
                {
                    child.GetParent().RemoveChild(child);
                    child.QueueFree();
                }
            }

            var requirementCell = _resourcePreloader.InstanceScene<InventoryCell>();
            requirementCell.SizeFlagsHorizontal = (int) SizeFlags.ShrinkCenter;
            _requirementsContainer.AddChild(requirementCell);
            var requirementItem = InventoryItem.FromItemId(questEventModel.ItemId);
            requirementItem.Amount = questEventModel.Required;
            requirementCell.SetInventoryItem(requirementItem);

            foreach (var reward in rewards)
            {
                var rewardCell = _resourcePreloader.InstanceScene<InventoryCell>();
                rewardCell.SizeFlagsHorizontal = (int) SizeFlags.ShrinkCenter;
                _rewardsContainer.AddChild(rewardCell);

                var item = InventoryItem.FromItemId(reward.ItemId);
                item.Amount = reward.Amount;
                rewardCell.SetInventoryItem(item);
            }
        }

        private void OnNextButtonPressed()
        {
            EmitSignal(nameof(NextButtonPressed));
        }

        private void OnNotYetButtonPressed()
        {
            EmitSignal(nameof(NotYetButtonPressed));
        }

        private void OnAcceptButtonPressed()
        {
            EmitSignal(nameof(QuestAcceptanceIndicated), true);
        }

        private void OnDeclineButtonPressed()
        {
            EmitSignal(nameof(QuestAcceptanceIndicated), false);
        }

        private void OnTurnInButtonPressed()
        {
            EmitSignal(nameof(QuestTurnInIndicated));
        }
    }
}