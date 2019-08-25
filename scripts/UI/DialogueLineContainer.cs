using GameFeel.Component.Subcomponent;
using GameFeel.Data.Model;
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

        [Export]
        private NodePath _dialogueLabelPath;
        [Export]
        private NodePath _acceptButtonPath;
        [Export]
        private NodePath _declineButtonPath;
        [Export]
        private NodePath _nextButtonPath;
        [Export]
        private NodePath _turnInButtonPath;
        [Export]
        private NodePath _notYetButtonPath;
        [Export]
        private NodePath _turnInContainerPath;
        [Export]
        private NodePath _inventoryCellPath;

        private Label _dialogueLabel;
        private Button _acceptButton;
        private Button _declineButton;
        private Button _nextButton;
        private Button _turnInButton;
        private Button _notYetButton;
        private Container _turnInContainer;
        private InventoryCell _inventoryCell;

        public override void _Ready()
        {
            this.SetNodesByDeclaredNodePaths();
            HideButtons();
            _nextButton.Connect("pressed", this, nameof(OnNextButtonPressed));
            _acceptButton.Connect("pressed", this, nameof(OnAcceptButtonPressed));
            _declineButton.Connect("pressed", this, nameof(OnDeclineButtonPressed));

            _nextButton.RectPivotOffset = _nextButton.RectSize / 2f;
            _acceptButton.RectPivotOffset = _acceptButton.RectSize / 2f;
            _declineButton.RectPivotOffset = _declineButton.RectSize / 2f;
        }

        public void DisplayLine(DialogueLine line)
        {
            HideButtons();
            _dialogueLabel.Text = line.Text;
            switch (line.LineContainerType)
            {
                case DialogueLine.LineType.NORMAL:
                    _nextButton.Show();
                    break;
                case DialogueLine.LineType.QUEST_ACCEPTANCE:
                    ShowQuestAcceptanceButtons();
                    break;
                case DialogueLine.LineType.TURN_IN:
                    SetupInventoryCell(line);
                    ShowTurnIn();
                    break;
            }

        }

        private void ShowQuestAcceptanceButtons()
        {
            HideButtons();
            _acceptButton.Show();
            _declineButton.Show();
        }

        private void ShowTurnIn()
        {
            HideButtons();
            _turnInContainer.Show();
            _turnInButton.Show();
            _notYetButton.Show();
        }

        private void HideButtons()
        {
            _acceptButton.Hide();
            _declineButton.Hide();
            _nextButton.Hide();
            _turnInButton.Hide();
            _notYetButton.Hide();
            _turnInContainer.Hide();
        }

        private void SetupInventoryCell(DialogueLine dialogueLine)
        {
            var parent = dialogueLine.GetParentOrNull<DialogueItem>();
            if (parent == null)
            {
                Logger.Error("Dialogue line had no parent of type " + nameof(DialogueItem));
                return;
            }
            var model = QuestTracker.GetActiveModel(parent.ActiveQuestModelId);
            if (model is QuestEventModel qem)
            {
                var metadata = MetadataLoader.LootItemIdToMetadata[qem.ItemId];
                var item = PlayerInventory.InventoryItemFromLootMetadata(metadata);
                item.Amount = qem.Required;
                _inventoryCell.SetInventoryItem(item);
            }
            else
            {
                Logger.Error("Could not parse model as an event model " + parent.ActiveQuestModelId);
            }
        }

        private void OnNextButtonPressed()
        {
            EmitSignal(nameof(NextButtonPressed));
        }

        private void OnAcceptButtonPressed()
        {
            EmitSignal(nameof(QuestAcceptanceIndicated), true);
        }

        private void OnDeclineButtonPressed()
        {
            EmitSignal(nameof(QuestAcceptanceIndicated), false);
        }
    }
}