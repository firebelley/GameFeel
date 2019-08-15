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
        }

        public virtual void LoadModel(QuestModel questModel)
        {

        }

        protected virtual void OnCloseRequest()
        {
            EmitSignal(nameof(CloseRequest), this);
        }
    }
}