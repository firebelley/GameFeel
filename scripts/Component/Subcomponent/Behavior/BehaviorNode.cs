using Godot;

namespace GameFeel.Component.Subcomponent.Behavior
{
    public abstract class BehaviorNode : Node
    {
        [Signal]
        public delegate void StatusUpdated(Status status);

        public enum Status
        {
            SUCCESS,
            FAIL,
            RUNNING
        }

        public override void _Ready()
        {
            SetProcess(false);
        }

        public override void _Process(float delta)
        {
            Tick();
        }

        protected BehaviorNode _processingNode;

        public abstract void Enter();
        protected abstract void Tick();

        protected void Leave(Status status)
        {
            SetProcess(false);
            EmitSignal(nameof(StatusUpdated), status);
        }
    }
}