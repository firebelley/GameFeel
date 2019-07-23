using Godot;
using GodotTools.Extension;

namespace GameFeel.Component
{
    [Tool]
    public class DamageDealerComponent : Node
    {
        [Export]
        public float Damage;
        [Export]
        private int _maxHits;
        [Export]
        private PackedScene _hitEffect;

        private int _hits;

        public void HandleHit(DamageReceiverComponent damageReceiverComponent)
        {
            if (_hits >= _maxHits) return;
            _hits++;
            SpawnHitEffect();

            damageReceiverComponent.HandleHit(this);

            if (_hits >= _maxHits)
            {
                GetOwner().GetFirstNodeOfType<ProjectileDeleterComponent>()?.Delete();
            }
        }

        private void SpawnHitEffect()
        {
            if (GetOwner() is Node2D owner && _hitEffect != null)
            {
                var death = _hitEffect.Instance() as Node2D;
                GameZone.EffectsLayer.AddChild(death);
                death.GlobalPosition = owner.GlobalPosition;
            }
        }
    }
}