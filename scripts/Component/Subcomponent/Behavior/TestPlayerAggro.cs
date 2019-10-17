using GameFeel.GameObject;
using GodotApiTools.Extension;

namespace GameFeel.Component.Subcomponent.Behavior
{
    public class TestPlayerAggro : BehaviorNode
    {
        protected override void InternalEnter()
        {
            if (_root.Blackboard.Aggressive || IsPlayerInAggroRange())
            {
                _root.Blackboard.Aggressive = true;
                Leave(Status.SUCCESS);
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

        private bool IsPlayerInAggroRange()
        {
            var player = GetTree().GetFirstNodeInGroup<Player>(Player.GROUP);
            return IsInstanceValid(player) && IsInstanceValid(_root.Blackboard.Owner) && _root.Blackboard.Owner.GlobalPosition.DistanceSquaredTo(player.GlobalPosition) < _root.Blackboard.AggroRange * _root.Blackboard.AggroRange;
        }
    }
}