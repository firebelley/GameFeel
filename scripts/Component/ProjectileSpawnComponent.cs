using Godot;
using GodotTools.Extension;

namespace GameFeel.Component
{
    public class ProjectileSpawnComponent : Position2D
    {
        [Export]
        private PackedScene _scene;
        [Export]
        private float _speed = 100f;
        [Export]
        private float _maxDistance = 100f;

        public void Spawn(Vector2 normalizedDirection)
        {
            var scene = _scene.Instance() as RigidBody2D;
            GameZone.EffectsLayer.AddChild(scene);
            scene.GlobalPosition = GlobalPosition;
            scene.LinearVelocity = normalizedDirection * _speed;

            if (_maxDistance > 0f)
            {
                scene.GetFirstNodeOfType<ProjectileDeleterComponent>()?.SetTravelDistance(_maxDistance);
            }
        }
    }
}