namespace GameFeel.Component.Subcomponent.Behavior
{
    public class Sequence : BehaviorNode
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
                    _children[_processingIndex].RequestEnter();
                }
            }
        }
    }
}