using System;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestNode : GraphNode
    {
        public QuestModel Model { get; protected set; } = new QuestModel();

        public class QuestModel
        {
            public string Id;
            public string DisplayName;
            public Vector2 NodePosition;

            public QuestModel()
            {
                Id = Guid.NewGuid().ToString();
            }
        }

        [Signal]
        public delegate void CloseRequest(QuestNode questNode);

        public override void _Ready()
        {
            Connect("close_request", this, nameof(OnCloseRequest));
            Connect("dragged", this, nameof(OnDragged));
            CallDeferred(nameof(Reposition));
        }

        public virtual void LoadModel(QuestModel questModel)
        {

        }

        private void Reposition()
        {
            Offset = Model.NodePosition;
        }

        protected virtual void OnCloseRequest()
        {
            EmitSignal(nameof(CloseRequest), this);
        }

        private void OnDragged(Vector2 from, Vector2 to)
        {
            Model.NodePosition = Offset;
        }
    }
}