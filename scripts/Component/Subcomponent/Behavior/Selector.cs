namespace GameFeel.Component.Subcomponent.Behavior
{
    public class Selector : BehaviorNode
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

        protected override void ChildStatusUpdated(Status status)
        {
            if (status == Status.FAIL)
            {
                _processingIndex++;
                if (_processingIndex >= _children.Count)
                {
                    Leave(Status.FAIL);
                }
                else
                {
                    _children[_processingIndex].Enter();
                }
            }
            else if (status == Status.SUCCESS)
            {
                Leave(Status.SUCCESS);
            }
        }
    }
}