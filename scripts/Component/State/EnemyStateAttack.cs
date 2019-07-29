using GameFeel.GameObject;
using Godot;
using GodotTools.Extension;
using GodotTools.Logic.Interface;

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
            _animatedSprite = GetNode<AnimatedSprite>(_animatedSpritePath);
            _attackIntentComponent = GetNode<AttackIntentComponent>(_attackIntentComponentPath);
            _projectileSpawnComponent = GetNode<ProjectileSpawnComponent>(_projectileSpawnComponentPath);

            if (_pursueStateNodePath != null)
            {
                _pursueState = GetNode(_pursueStateNodePath) as IStateExector;
            }

            if (_animatedSprite != null)
            {
                _animatedSprite.Connect("animation_finished", this, nameof(OnAnimationFinished));
            }
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
            if (_animatedSprite.Animation == EnemyAIComponent.META_ANIM_ATTACK)
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