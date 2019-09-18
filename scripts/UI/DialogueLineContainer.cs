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
        private NodePath _completeContainerPath;
        [Export]
        private NodePath _inventoryCellPath;

        private Label _dialogueLabel;
        private Button _acceptButton;
        private Button _declineButton;
        private Button _nextButton;
        private Button _completeButton;
        private Button _notYetButton;
        private Container _completeContainer;
        private InventoryCell _inventoryCell;

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
            _completeContainer.Show();
            _notYetButton.Show();

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
            _completeContainer.Hide();
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