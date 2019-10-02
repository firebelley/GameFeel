using GameFeel.Component;
using Godot;
using GodotTools.Extension;

namespace GameFeel.GameObject.Loot
{
    public class Equipment : Node2D
    {
        private const string ANIM_ATTACK = "attack";

        [Export(PropertyHint.Enum, "1,2,3,4")]
        public int SlotIndex { get; private set; }

        [Export]
        public float FireRate { get; private set; } = 5f; // projectiles per second

        [Export]
        public float ManaCost { get; private set; } = 1f;

        private ProjectileSpawnComponent _projectileSpawnComponent;
        private Timer _fireRateTimer;
        private AnimationPlayer _animationPlayer;

        public override void _Ready()
        {
            var player = GetTree().GetFirstNodeInGroup<Player>(Player.GROUP);
            if (player != null)
            {
                player.Connect(nameof(Player.Attacking), this, nameof(OnPlayerAttacking));
            }
            _projectileSpawnComponent = GetNode<ProjectileSpawnComponent>("ProjectileSpawnComponent");
            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _fireRateTimer = GetNode<Timer>("FireRateTimer");
            _fireRateTimer.WaitTime = 1f / FireRate;
        }

        private void Fire()
        {
            var direction = Vector2.Right.Rotated(GlobalRotation * Mathf.Sign(GlobalScale.y));
            _projectileSpawnComponent.Spawn(direction);
            if (_animationPlayer.IsPlaying())
            {
                _animationPlayer.Stop(true);
            }
            _animationPlayer.Play(ANIM_ATTACK);
        }

        private void OnPlayerAttacking(Player player)
        {
            if (_fireRateTimer.IsStopped())
            {
                if (player.Mana >= ManaCost)
                {
                    player.RemoveMana(ManaCost);
                    Fire();
                    _fireRateTimer.Start();
                }
            }
        }
    }
}