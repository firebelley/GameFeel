namespace GameFeel.Component.Subcomponent.Behavior
{
    public class Sequence : BehaviorNode
    {
        private int _processingIndex = 0;

        protected override void InternalEnter()
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

        protected override void InternalLeave()
        {

        }

        protected override void ChildStatusUpdated(Status status, BehaviorNode behaviorNode)
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