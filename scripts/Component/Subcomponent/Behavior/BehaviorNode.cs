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

        protected BehaviorNode _processingNode;

        public override void _Ready()
        {
            SetProcess(false);
        }

        public override void _Process(float delta)
        {
            Tick();
        }

        public void RequestEnter()
        {
            CallDeferred(nameof(Enter));
        }

        protected abstract void Tick();
        protected abstract void Enter();

        protected virtual void Leave(Status status)
        {
            SetProcess(false);
            EmitSignal(nameof(StatusUpdated), status);
        }
    }
}