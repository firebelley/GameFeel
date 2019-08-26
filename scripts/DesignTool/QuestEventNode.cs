using GameFeel.Data.Model;
using GameFeel.Singleton;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestEventNode : QuestNode
    {
        private LineEdit _promptLineEdit;
        private LineEdit _idLineEdit;

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
            _idLineEdit = GetNode<LineEdit>("VBoxContainer/IdContainer/LineEdit");
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
            _idLineEdit.Text = Model.Id;
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