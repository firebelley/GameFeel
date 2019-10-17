using Godot;
using GodotApiTools.Logic.Interface;

namespace GameFeel.Component.State
{
    public class EnemyStateAttackPreparation : EnemyState
    {
        [Export]
        private NodePath _attackStateNodePath;
        [Export]
        private NodePath _animatedSpritePath;
        [Export]
        private NodePath _attackIntentComponentPath;
        [Export]
        private float _preparationTime = .25f;

        private IStateExector _attackState;

        private AttackIntentComponent _attackIntentComponent;
        private AnimatedSprite _animatedSprite;

        private Timer _timer;

        public override void _Ready()
        {
            base._Ready();
            _timer = GetNode<Timer>("Timer");

            _attackIntentComponent = GetNode<AttackIntentComponent>(_attackIntentComponentPath);
            _animatedSprite = GetNode<AnimatedSprite>(_animatedSpritePath);

            if (_attackStateNodePath != null)
            {
                _attackState = GetNode(_attackStateNodePath) as IStateExector;
            }
        }

        public override void StateActive()
        {
            if (_timer.IsStopped())
            {
                _parent.StateMachine.ChangeState(_attackState);
            }
        }

        public override void StateEntered()
        {
            _timer.WaitTime = _preparationTime;
            _timer.Start();
            _attackIntentComponent.Play();
            _animatedSprite.Play(EnemyAIComponent.META_ANIM_IDLE);
        }

        public override void StateLeft()
        {

        }
    }
}