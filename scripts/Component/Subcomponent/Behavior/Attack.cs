using Godot;

namespace GameFeel.Component.Subcomponent.Behavior
{
    public class Attack : BehaviorNode
    {
        [Export]
        private NodePath _projectileSpawnComponentPath;

        private ProjectileSpawnComponent _projectileSpawnComponent;

        public override void _Ready()
        {
            base._Ready();
            _projectileSpawnComponent = GetNodeOrNull<ProjectileSpawnComponent>(_projectileSpawnComponentPath ?? string.Empty);
        }

        protected override void InternalEnter()
        {
            _projectileSpawnComponent.SpawnToPosition(_root.Blackboard.AttackTargetPosition);
            Leave(Status.SUCCESS);
        }

        protected override void Tick()
        {

        }

        protected override void InternalLeave() { }

    }
}