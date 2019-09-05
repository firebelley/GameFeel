using GameFeel.Singleton;
using Godot;

namespace GameFeel.Component.Subcomponent.Behavior
{
    public class DispatchEntityEngagedEvent : BehaviorNode
    {
        [Export]
        private NodePath _entityDataComponentPath;

        private bool _eventSent;
        private EntityDataComponent _entityDataComponent;

        public override void _Ready()
        {
            base._Ready();
            _entityDataComponent = GetNodeOrNull<EntityDataComponent>(_entityDataComponentPath ?? string.Empty);
        }

        protected override void InternalEnter()
        {
            if (!_eventSent)
            {
                _eventSent = true;
                GameEventDispatcher.DispatchEntityEngagedEvent(_entityDataComponent.Id);
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