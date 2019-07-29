using GameFeel.GameObject;
using Godot;
using GodotTools.Extension;
using GodotTools.Logic.Interface;

namespace GameFeel.Component.State
{
    public class EnemyStateAttack : Node, IStateExector
    {
        private const string ANIM_ATTACK = "attack";

        [Export]
        private NodePath _pursueStateNodePath;

        private EnemyAIComponent _parent;
        private AnimatedSprite _animatedSprite;
        private AttackIntentComponent _attackIntentComponent;
        private ProjectileSpawnComponent _projectileSpawnComponent;

        private IStateExector _pursueState;

        public override void _Ready()
        {
            _parent = GetParent() as EnemyAIComponent;
            _animatedSprite = _parent?.GetOwner()?.GetFirstNodeOfType<AnimatedSprite>();
            _attackIntentComponent = _parent?.GetOwner()?.GetFirstNodeOfType<AttackIntentComponent>();
            _projectileSpawnComponent = _parent?.GetOwner()?.GetFirstNodeOfType<ProjectileSpawnComponent>();

            if (_pursueStateNodePath != null)
            {
                _pursueState = GetNode(_pursueStateNodePath) as IStateExector;
            }

            if (_animatedSprite != null)
            {
                _animatedSprite.Connect("animation_finished", this, nameof(OnAnimationFinished));
            }
        }

        public void StateActive()
        {

        }

        public void StateEntered()
        {
            _animatedSprite?.Play(ANIM_ATTACK);
        }

        public void StateLeft()
        {
            _attackIntentComponent?.Stop();
        }

        private void OnAnimationFinished()
        {
            if (_animatedSprite.Animation == ANIM_ATTACK)
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