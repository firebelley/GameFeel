using System;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestNode : GraphNode
    {
        public class QuestModel
        {
            public string Id;
            public string DisplayName;
        }

        [Signal]
        public delegate void CloseRequest(QuestNode questNode);

        public override void _Ready()
        {
            Connect("close_request", this, nameof(OnCloseRequest));
        }

        public virtual QuestModel GetSaveModel()
        {
            throw new NotImplementedException();
        }

        protected virtual void OnCloseRequest()
        {
            EmitSignal(nameof(CloseRequest), this);
        }
    }
}