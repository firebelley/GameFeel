using GameFeel.GameObject;
using Godot;
using GodotApiTools.Extension;
using GodotApiTools.Logic.Interface;

namespace GameFeel.Component.State
{
    public class EnemyStateAttack : EnemyState
    {
        [Export]
        private NodePath _pursueStateNodePath;
        [Export]
        private NodePath _animatedSpritePath;
        [Export]
        private NodePath _attackIntentComponentPath;
        [Export]
        private NodePath _projectileSpawnComponentPath;

        private AnimatedSprite _animatedSprite;
        private AttackIntentComponent _attackIntentComponent;
        private ProjectileSpawnComponent _projectileSpawnComponent;

        private IStateExector _pursueState;

        public override void _Ready()
        {
            base._Ready();
            _animatedSprite = GetNodeOrNull<AnimatedSprite>(_animatedSpritePath ?? string.Empty);
            _projectileSpawnComponent = GetNodeOrNull<ProjectileSpawnComponent>(_projectileSpawnComponentPath ?? string.Empty);
            _pursueState = GetNodeOrNull(_pursueStateNodePath ?? string.Empty) as IStateExector;
            _attackIntentComponent = GetNodeOrNull<AttackIntentComponent>(_attackIntentComponentPath ?? string.Empty);
            _animatedSprite?.Connect("animation_finished", this, nameof(OnAnimationFinished));
        }

        public override void StateActive()
        {

        }

        public override void StateEntered()
        {
            _animatedSprite?.Play(EnemyAIComponent.META_ANIM_ATTACK);
        }

        public override void StateLeft()
        {
            _attackIntentComponent?.Stop();
        }

        private void OnAnimationFinished()
        {
            if (_animatedSprite?.Animation == EnemyAIComponent.META_ANIM_ATTACK)
            {
                _parent.StateMachine.ChangeState(_pursueState);

                var player = GetTree().GetFirstNodeInGroup<Player>(Player.GROUP);
                var toPos = Vector2.Zero;
                if (player != null)
                {
                    toPos = player.GetFirstNodeOfType<DamageReceiverComponent>()?.GlobalPosition ?? player.GlobalPosition;
                }
                _projectileSpawnComponent?.SpawnToPosition(toPos);
            }
        }
    }
}