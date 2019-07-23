using GameFeel.Component;
using Godot;

namespace GameFeel.GameObject
{
    public class Book : Sprite
    {
        private ProjectileSpawnComponent _projectileSpawnComponent;

        public override void _Ready()
        {
            if (GetOwner() is Player p)
            {
                p.Connect(nameof(Player.Attack), this, nameof(OnPlayerAttack));
            }

            _projectileSpawnComponent = GetNode<ProjectileSpawnComponent>("ProjectileSpawnComponent");
        }

        private void OnPlayerAttack(Player p)
        {
            if (p.Mana >= 1f)
            {
                var direction = Vector2.Right.Rotated(GlobalRotation * Mathf.Sign(GlobalScale.y));
                _projectileSpawnComponent.Spawn(direction);
                p.RemoveMana(1f);
            }
        }
    }
}