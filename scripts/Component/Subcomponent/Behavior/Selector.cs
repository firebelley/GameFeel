namespace GameFeel.Component.Subcomponent.Behavior
{
    public class Selector : BehaviorNode
    {
        private int _processingIndex = 0;

        protected override void Enter()
        {
            _processingIndex = 0;
            if (_children.Count > 0)
            {
                _children[_processingIndex].RequestEnter();
            }
            else
            {
                Leave(Status.FAIL);
            }
        }

        protected override void Tick()
        {

        }

        protected override void OnChildStatusUpdated(Status status)
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
                    _children[_processingIndex].RequestEnter();
                }
            }
            else if (status == Status.SUCCESS)
            {
                Leave(Status.SUCCESS);
            }
        }
    }
}