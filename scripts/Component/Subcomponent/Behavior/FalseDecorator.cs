namespace GameFeel.Component.Subcomponent.Behavior
{
    public class FalseDecorator : BehaviorNode
    {
        protected override void InternalEnter()
        {
            if (_children.Count == 1)
            {
                _children[0].Enter();
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
            Leave(Status.FAIL);
        }
    }
}