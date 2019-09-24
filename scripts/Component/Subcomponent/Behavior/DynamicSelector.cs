using System.Collections.Generic;

namespace GameFeel.Component.Subcomponent.Behavior
{
    public class DynamicSelector : BehaviorNode
    {
        private List<BehaviorNode> _retryNodes = new List<BehaviorNode>();
        private int _tryIndex = 0;

        protected override void InternalEnter()
        {
            SetProcess(true);

            _retryNodes.Clear();
            _tryIndex = 0;
            foreach (var child in _children)
            {
                _retryNodes.Add(child);
            }

            if (_children.Count == 0)
            {
                Leave(Status.FAIL);
            }
        }

        protected override void Tick()
        {
            BehaviorNode runningChild = GetFirstRunningChild();
            int childIndex = runningChild?.GetIndex() ?? _retryNodes.Count;
            if (runningChild != null && _tryIndex > runningChild.GetIndex())
            {
                _tryIndex = 0;
            }

            var tryNode = FindFirstTryNodeBeforeIndex(childIndex);
            if (tryNode != null)
            {
                _retryNodes[_tryIndex] = null;
                tryNode.Enter();
                if (tryNode.IsRunning)
                {
                    runningChild?.Terminate();
                }
            }
            else
            {
                _tryIndex = 0;
            }
        }

        protected override void InternalLeave()
        {
            _retryNodes.Clear();
        }

        protected override void ChildStatusUpdated(Status status, BehaviorNode behaviorNode)
        {
            if (status == Status.FAIL)
            {
                _tryIndex++;
                _retryNodes[behaviorNode.GetIndex()] = behaviorNode;
            }
            else if (status == Status.SUCCESS)
            {
                EmitSignal(nameof(Aborted));
                Leave(Status.SUCCESS);
            }
        }

        private BehaviorNode GetFirstRunningChild()
        {
            var childIndex = GetFirstRunningChildIndex();
            if (childIndex > -1)
            {
                return _children[childIndex];
            }
            return null;
        }

        private BehaviorNode FindFirstTryNodeBeforeIndex(int beforeIdx)
        {
            while (_tryIndex < beforeIdx && _retryNodes[_tryIndex] == null)
            {
                _tryIndex++;
            }
            if (_tryIndex < beforeIdx)
            {
                return _children[_tryIndex];
            }
            return null;
        }
    }
}