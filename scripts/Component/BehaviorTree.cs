using GameFeel.Component.Subcomponent.Behavior;
using Godot;
using GodotTools.Extension;

namespace GameFeel.Component
{
    public class BehaviorTree : Node
    {
        private BehaviorNode _nextToEnter;

        public override void _Ready()
        {
            BehaviorNode first = null;
            foreach (var child in this.GetChildren<BehaviorNode>())
            {
                if (first == null)
                {
                    first = child;
                }
                child.Connect(nameof(BehaviorNode.StatusUpdated), this, nameof(OnBehaviorNodeStatusUpdated));
            }
            first?.Enter();
        }

        public override void _Process(float delta)
        {
            if (IsInstanceValid(_nextToEnter))
            {
                var next = _nextToEnter;
                _nextToEnter = null;
                next.Enter();
            }
        }

        private void OnBehaviorNodeStatusUpdated(BehaviorNode.Status status)
        {
            _nextToEnter = this.GetChildren<BehaviorNode>() [0];
        }
    }
}