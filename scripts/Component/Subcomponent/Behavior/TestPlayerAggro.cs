using GameFeel.GameObject;
using Godot;
using GodotTools.Extension;

namespace GameFeel.Component.Subcomponent.Behavior
{
    public class TestPlayerAggro : BehaviorNode
    {
        [Export]
        private int _aggroDistance = 50;

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
            return IsInstanceValid(player) && IsInstanceValid(_root.Blackboard.Owner) && _root.Blackboard.Owner.GlobalPosition.DistanceSquaredTo(player.GlobalPosition) < _aggroDistance * _aggroDistance;
        }
    }
}