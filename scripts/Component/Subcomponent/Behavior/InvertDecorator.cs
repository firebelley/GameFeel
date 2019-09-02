namespace GameFeel.Component.Subcomponent.Behavior
{
    public class InvertDecorator : BehaviorNode
    {
        public override void Enter()
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

        protected override void OnChildStatusUpdated(Status status)
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