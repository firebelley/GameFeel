using GameFeel.UI;
using Godot;
using GodotTools.Extension;
using GodotTools.Util;

namespace GameFeel.Component.Subcomponent
{
    public class DialogueItem : Node
    {
        [Signal]
        public delegate void LinePresented(DialogueItem dialogueItem, DialogueLine dialogueLine);
        [Signal]
        public delegate void LinesFinished();

        [Export]
        public string Title { get; private set; }

        [Export]
        public string RequiredQuestStageId { get; private set; }

        [Export]
        public bool ShowImmediately { get; private set; }

        // start a quest on dialogue finish
        [Export]
        private NodePath _questStarterComponentPath;

        private QuestStarterComponent _questStarterComponent;

        public override void _Ready()
        {
            if (_questStarterComponentPath != null)
            {
                _questStarterComponent = GetNode<QuestStarterComponent>(_questStarterComponentPath);
            }
        }

        public void StartLines()
        {
            PresentLine(0);
            CheckCompletion(0);
        }

        public void ConnectDialogueUISignals(DialogueUI dialogueUI)
        {
            this.DisconnectAllSignals(dialogueUI);
            dialogueUI.Connect(nameof(DialogueUI.LineAdvanceRequested), this, nameof(OnLineAdvanceRequested));
        }

        public bool LineStartsQuest(int idx)
        {
            return GetChildCount() - 1 == idx && IsInstanceValid(_questStarterComponent);
        }

        private bool CheckCompletion(int childIdx)
        {
            if (GetChildCount() == childIdx)
            {
                _questStarterComponent?.StartQuest();
                EmitSignal(nameof(LinesFinished));
                return true;
            }
            return false;
        }

        private void PresentLine(int idx)
        {
            if (idx < GetChildCount())
            {
                var line = GetChild<DialogueLine>(idx);
                EmitSignal(nameof(LinePresented), this, line);
            }
            else
            {
                Logger.Error("Tried to present dialogue line that was out of range with idx " + idx + " for owner " + GetOwner().GetName());
            }
        }

        private void OnLineAdvanceRequested(int toIdx)
        {
            if (!CheckCompletion(toIdx))
            {
                PresentLine(toIdx);
            }
        }
    }
}