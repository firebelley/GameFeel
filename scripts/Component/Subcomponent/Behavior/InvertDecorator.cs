namespace GameFeel.Component.Subcomponent.Behavior
{
    public class InvertDecorator : BehaviorNode
    {
        protected override void InternalEnter()
        {
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

        }

        protected override void InternalLeave()
        {

        }

        protected override void ChildStatusUpdated(Status status)
        {
            if (status == Status.SUCCESS)
            {
                Leave(Status.FAIL);
            }
            else if (status == Status.FAIL)
            {
                Leave(Status.SUCCESS);
            }
        }
    }
}