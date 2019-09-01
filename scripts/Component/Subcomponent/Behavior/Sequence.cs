using System.Collections.Generic;
using Godot;
using GodotTools.Extension;

namespace GameFeel.Component.Subcomponent.Behavior
{
    public class Sequence : BehaviorNode
    {
        private List<BehaviorNode> _children;
        private int _processingIndex = 0;

        public override void _Ready()
        {
            base._Ready();
            _children = this.GetChildren<BehaviorNode>();
            foreach (var child in _children)
            {
                child.Connect(nameof(BehaviorNode.StatusUpdated), this, nameof(OnStatusUpdated));
            }
        }

        public override void Enter()
        {
            _processingIndex = 0;
            if (_children.Count > 0)
            {
                _children[_processingIndex].Enter();
            }
            else
            {
                Leave(Status.FAIL);
            }
        }

        protected override void Tick()
        {

        }

        private void OnStatusUpdated(Status status)
        {
            if (status == Status.FAIL)
            {
                Leave(Status.FAIL);
            }
            else if (status == Status.SUCCESS)
            {
                _processingIndex++;
                if (_processingIndex >= _children.Count)
                {
                    Leave(Status.SUCCESS);
                }
                else
                {
                    _children[_processingIndex].Enter();
                }
            }
        }
    }
}