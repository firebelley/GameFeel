using Godot;

namespace GameFeel.DesignTool
{
    public class QuestNode : GraphNode
    {
        [Signal]
        public delegate void CloseRequest(QuestNode questNode);

        public override void _Ready()
        {
            Connect("close_request", this, nameof(OnCloseRequest));
        }

        protected virtual void OnCloseRequest()
        {
            EmitSignal(nameof(CloseRequest), this);
        }
    }
}