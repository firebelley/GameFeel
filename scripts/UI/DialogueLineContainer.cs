using GameFeel.Component.Subcomponent;
using Godot;
using GodotTools.Extension;

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

        private Label _dialogueLabel;
        private Button _acceptButton;
        private Button _declineButton;
        private Button _nextButton;

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
                    break;
            }

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