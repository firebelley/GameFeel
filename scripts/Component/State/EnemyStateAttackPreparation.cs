using Godot;
using GodotTools.Extension;
using GodotTools.Logic.Interface;

namespace GameFeel.Component.State
{
    public class EnemyStateAttackPreparation : Node, IStateExector
    {
        private const string ANIM_IDLE = "idle";

        [Export]
        private NodePath _attackStateNodePath;

        private EnemyAIComponent _parent;
        private IStateExector _attackState;

        private AttackIntentComponent _attackIntentComponent;
        private AnimatedSprite _animatedSprite;

        private Timer _timer;

        public override void _Ready()
        {
            _parent = GetParent() as EnemyAIComponent;
            _timer = GetNode<Timer>("Timer");

            _attackIntentComponent = _parent?.Owner?.GetFirstNodeOfType<AttackIntentComponent>();
            _animatedSprite = _parent?.Owner?.GetFirstNodeOfType<AnimatedSprite>();

            if (_attackStateNodePath != null)
            {
                _attackState = GetNode(_attackStateNodePath) as IStateExector;
            }
        }

        public void StateActive()
        {
            if (_timer.IsStopped())
            {
                _parent.StateMachine.ChangeState(_attackState);
            }
        }

        public void StateEntered()
        {
            _timer.WaitTime = .25f;
            _timer.Start();
            _attackIntentComponent.Play();
            _animatedSprite.Play(ANIM_IDLE);
        }

        public void StateLeft()
        {

        }
    }
}