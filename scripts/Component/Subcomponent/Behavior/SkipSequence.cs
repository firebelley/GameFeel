namespace GameFeel.Component.Subcomponent.Behavior
{
    /// <summary>
    /// If the first child returns a success or fail status, halt the rest of the children.
    /// If the first child is running, then proceed onto the others like a standard sequence.
    /// Only returns a status of success if all children succeed, like a standard sequence.
    /// </summary>
    public class SkipSequence : BehaviorNode
    {
        private int _processingIndex = 0;

        protected override void InternalEnter()
        {
            _processingIndex = 0;
            if (_children.Count > 0)
            {
                _children[0].Enter();
                if (_children[0].IsRunning)
                {
                    EnterNextChild();
                }
            }
            else
            {
                Leave(Status.SUCCESS);
            }
        }

        protected override void InternalLeave()
        {

        }

        protected override void Tick()
        {

        }

        protected override void ChildStatusUpdated(Status status, BehaviorNode behaviorNode)
        {
            if (behaviorNode == _children[0])
            {
                EmitSignal(nameof(Aborted));
                Leave(status);
            }
            else if (status == Status.FAIL)
            {
                EmitSignal(nameof(Aborted));
                Leave(status);
            }
            else if (status == Status.SUCCESS)
            {
                EnterNextChild();
            }
        }

        private void EnterNextChild()
        {
            _processingIndex++;
            if (_processingIndex < _children.Count)
            {
                _children[_processingIndex].Enter();
            }
        }
    }
}