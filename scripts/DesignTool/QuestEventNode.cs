using GameFeel.Singleton;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestEventNode : QuestNode
    {
        public class QuestEventModel : QuestNode.QuestModel
        {
            public string EventId;
            public string ItemId;
            public int Required;
        }

        public override void _Ready()
        {
            base._Ready();
            Model = new QuestEventModel();
            Model.DisplayName = "event";
            CallDeferred(nameof(SetNodeTitle));
        }

        public override void LoadModel(QuestModel questModel)
        {
            Model = (QuestEventModel) questModel;
            UpdateControls();
        }

        protected virtual void UpdateControls()
        {

        }

        private void SetNodeTitle()
        {
            Title = GameEventDispatcher.GameEventMapping[((QuestEventModel) Model).EventId].DisplayName;
        }
    }
}