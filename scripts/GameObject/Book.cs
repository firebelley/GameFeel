using Godot;
using GodotTools.Extension;

namespace GameFeel.GameObject
{
    public class Book : Sprite
    {
        private ResourcePreloader _resourcePreloader;

        public override void _Ready()
        {
            if (GetOwner() is Player p)
            {
                p.Connect(nameof(Player.Attack), this, nameof(OnPlayerAttack));
            }

            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
        }

        private void OnPlayerAttack(Player p)
        {
            if (p.Mana >= 1f)
            {
                var fireball = _resourcePreloader.InstanceScene<Fireball>();
                GameWorld.EffectsLayer.AddChild(fireball);

                var position = GlobalPosition;
                var direction = Vector2.Right.Rotated(GlobalRotation * Mathf.Sign(GlobalScale.y));
                fireball.SetDirection(direction);
                fireball.GlobalPosition = position;
                p.RemoveMana(1f);
            }
        }
    }
}