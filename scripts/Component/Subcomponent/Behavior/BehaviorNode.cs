using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotTools.Extension;

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

        protected List<BehaviorNode> _children;

        public override void _Ready()
        {
            SetProcess(false);
            _children = this.GetChildren<BehaviorNode>().Where(x => IsInstanceValid(x)).ToList();
            foreach (var child in _children)
            {
                child.Connect(nameof(StatusUpdated), this, nameof(OnChildStatusUpdated));
            }
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

        protected virtual void OnChildStatusUpdated(Status status) { }
    }
}