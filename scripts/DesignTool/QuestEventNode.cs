using GameFeel.Data.Model;
using GameFeel.Singleton;

namespace GameFeel.DesignTool
{
    public class QuestEventNode : QuestNode
    {
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