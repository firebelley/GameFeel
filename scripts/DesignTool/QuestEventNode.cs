using GameFeel.Data.Model;
using GameFeel.Singleton;

namespace GameFeel.DesignTool
{
    public class QuestEventNode : QuestNode
    {
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
            Model = new QuestEventModel();
            Model.DisplayName = "event";
            CallDeferred(nameof(SetNodeTitle));
        }

        public override void LoadModel(QuestModel questModel)
        {
            base.LoadModel(questModel);
            Model = (QuestEventModel) questModel;
        }

        private void SetNodeTitle()
        {
            Title = GameEventDispatcher.GameEventMapping[((QuestEventModel) Model).EventId].DisplayName;
        }
    }
}