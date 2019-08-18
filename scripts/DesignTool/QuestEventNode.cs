using GameFeel.Data.Model;
using GameFeel.Singleton;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestEventNode : QuestNode
    {
        private LineEdit _promptLineEdit;

        public new QuestEventModel Model
        {
            get
            {
                return (QuestEventModel) base.Model;
            }
            set
            {
                base.Model = value;
            }
        }

        public override void _Ready()
        {
            base._Ready();

            _promptLineEdit = GetNode<LineEdit>("VBoxContainer/PromptContainer/LineEdit");
            _promptLineEdit.Connect("text_changed", this, nameof(OnTextChanged));

            Model = new QuestEventModel();
            Model.DisplayName = "event";
            CallDeferred(nameof(SetNodeTitle));
        }

        public override void LoadModel(QuestModel questModel)
        {
            base.LoadModel(questModel);
            Model = (QuestEventModel) questModel;
        }

        protected override void UpdateControls()
        {
            _promptLineEdit.Text = Model.PromptText;
        }

        private void SetNodeTitle()
        {
            Title = GameEventDispatcher.GameEventMapping[((QuestEventModel) Model).EventId].DisplayName;
        }

        private void OnTextChanged(string newText)
        {
            Model.PromptText = _promptLineEdit.Text;
        }
    }
}