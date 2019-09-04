using GameFeel.GameObject;
using Godot;
using GodotTools.Extension;

namespace GameFeel.Component.Subcomponent.Behavior
{
    public class AcquireTarget : BehaviorNode
    {
        protected override void InternalEnter()
        {
            var player = GetTree().GetFirstNodeInGroup<Player>(Player.GROUP);
            if (player != null)
            {
                _root.Blackboard.AttackTargetPosition = player.GetFirstNodeOfType<DamageReceiverComponent>()?.GlobalPosition ?? Vector2.Zero;
            }
            Leave(Status.SUCCESS);
        }

        protected override void Tick()
        {

        }

        protected override void InternalLeave()
        {

        }
    }
}