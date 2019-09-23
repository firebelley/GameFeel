namespace GameFeel.Component.Subcomponent.Behavior
{
    public class StopMovement : BehaviorNode
    {
        protected override void InternalEnter()
        {
            _root.Blackboard.PathfindComponent?.ClearPath();
            Leave(Status.SUCCESS);
        }

        protected override void InternalLeave()
        {

        }

        protected override void Tick()
        {

        }
    }
}