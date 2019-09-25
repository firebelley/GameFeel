using Godot;
using GodotTools.Extension;

namespace GameFeel.Component
{
    [Tool]
    public class ProjectileSpawnComponent : Position2D
    {
        [Signal]
        public delegate void AllSpawned();

        [Export(PropertyHint.Layers2dPhysics)]
        private int _collisionMask;
        [Export]
        private PackedScene _scene;
        [Export]
        private float _damageOverride;
        [Export]
        private float _speed = 100f;
        [Export]
        private float _maxDistance = 100f;
        [Export]
        private int _number = 1;
        [Export]
        private float _angleDelta;
        [Export]
        private float _delayPerSpawn;

        private Timer _spawnTimer;
        private Vector2 _targetDirection;
        private int _totalSpawned;

        public override void _Ready()
        {
            _spawnTimer = GetNode<Timer>("SpawnTimer");
            _spawnTimer.Connect("timeout", this, nameof(OnSpawnTimerTimeout));
        }

        public override string _GetConfigurationWarning()
        {
            if (_collisionMask == 0)
            {
                return "Be sure to set a collision mask for the projectile.";
            }
            return string.Empty;
        }

        public void SpawnToPosition(Vector2 targetPos)
        {
            _targetDirection = (targetPos - GlobalPosition).Normalized();
            Setup();
            Spawn();
        }

        public void Spawn(Vector2 targetDir)
        {
            _targetDirection = targetDir.Normalized();
            Setup();
            Spawn();
        }

        private void Setup()
        {
            _totalSpawned = 0;
            SetInitialTargetAngle();
        }

        private void UpdateTargetAngle()
        {
            _targetDirection = _targetDirection.Rotated(Mathf.Deg2Rad(-_angleDelta));
        }

        private void SetInitialTargetAngle()
        {
            var totalAngleDiff = Mathf.Deg2Rad(_angleDelta * (_number - 1));
            _targetDirection = _targetDirection.Rotated(totalAngleDiff / 2f);
        }

        private void Spawn()
        {
            var scene = _scene.Instance() as RigidBody2D;
            GameZone.EffectsLayer.AddChild(scene);
            scene.GlobalPosition = GlobalPosition;
            scene.LinearVelocity = _targetDirection * _speed;
            scene.CollisionMask = _collisionMask;

            if (_maxDistance > 0f)
            {
                scene.GetFirstNodeOfType<ProjectileDeleterComponent>()?.SetTravelDistance(_maxDistance);
            }

            if (_damageOverride > 0f)
            {
                var damageDealerComponent = scene.GetFirstNodeOfType<DamageDealerComponent>();
                if (damageDealerComponent != null)
                {
                    damageDealerComponent.Damage = _damageOverride;
                }
            }

            _totalSpawned++;
            UpdateTargetAngle();

            if (_totalSpawned < _number)
            {
                if (_delayPerSpawn == 0f)
                {
                    Spawn();
                }
                else
                {
                    _spawnTimer.WaitTime = _delayPerSpawn;
                    _spawnTimer.Start();
                }
            }
            else
            {
                EmitSignal(nameof(AllSpawned));
            }
        }

        private void OnSpawnTimerTimeout()
        {
            Spawn();
        }
    }
}