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
        }

        public void DisplayLine(string line)
        {
            HideButtons();
            _dialogueLabel.Text = line;
            _nextButton.Show();
        }

        public void ShowQuestAcceptanceButtons()
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