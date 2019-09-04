using System.Collections.Generic;

namespace GameFeel.Component.Subcomponent.Behavior
{
    public class DynamicSelector : BehaviorNode
    {
        private int _processingIndex = 0;
        private List<BehaviorNode> _failedNodes = new List<BehaviorNode>();

        protected override void InternalEnter()
        {
            _processingIndex = 0;
            SetProcess(true);
            if (_children.Count > 0)
            {
                _children[0].Enter();
            }
            else
            {
                Leave(Status.FAIL);
            }
        }

        protected override void Tick()
        {
            var failures = _failedNodes.ToArray();
            _failedNodes.Clear();
            foreach (var child in failures)
            {
                child.Enter();
            }
        }

        protected override void InternalLeave()
        {
            _failedNodes.Clear();
        }

        protected override void ChildStatusUpdated(Status status, BehaviorNode behaviorNode)
        {
            if (status == Status.FAIL)
            {
                _failedNodes.Add(behaviorNode);
                if (_processingIndex < _children.Count - 1)
                {
                    _processingIndex++;
                    var nextChild = _children[_processingIndex];
                    nextChild.Enter();
                }
            }
            else if (status == Status.SUCCESS)
            {
                EmitSignal(nameof(Aborted));
                Leave(Status.SUCCESS);
            }
        }
    }
}