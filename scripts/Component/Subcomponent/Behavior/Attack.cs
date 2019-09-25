using Godot;

namespace GameFeel.Component.Subcomponent.Behavior
{
    public class Attack : BehaviorNode
    {
        [Export]
        private NodePath _projectileSpawnComponentPath;
        [Export]
        private bool _waitAllSpawned = false;

        private ProjectileSpawnComponent _projectileSpawnComponent;

        public override void _Ready()
        {
            base._Ready();
            _projectileSpawnComponent = GetNodeOrNull<ProjectileSpawnComponent>(_projectileSpawnComponentPath ?? string.Empty);
            _projectileSpawnComponent.Connect(nameof(ProjectileSpawnComponent.AllSpawned), this, nameof(OnAllSpawned));
        }

        protected override void InternalEnter()
        {
            _projectileSpawnComponent.SpawnToPosition(_root.Blackboard.AttackTargetPosition);
            if (!_waitAllSpawned)
            {
                Leave(Status.SUCCESS);
            }
        }

        protected override void Tick()
        {

        }

        protected override void InternalLeave() { }

        private void OnAllSpawned()
        {
            if (_waitAllSpawned)
            {
                Leave(Status.SUCCESS);
            }
        }
    }
}