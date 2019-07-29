using Godot;
using GodotTools.Logic.Interface;

namespace GameFeel.Component.State
{
    public class EnemyStateSpawn : EnemyState
    {
        [Export]
        private NodePath _wanderStateNodePath;
        [Export]
        private NodePath _animatedSpritePath;
        [Export]
        private NodePath _spawnAnimationPlayerPath;

        private IStateExector _wanderState;

        private AnimatedSprite _animatedSprite;
        private AnimationPlayer _spawnAnimationPlayer;

        public override void _Ready()
        {
            base._Ready();
            if (_wanderStateNodePath != null)
            {
                _wanderState = GetNode(_wanderStateNodePath) as IStateExector;
            }

            _animatedSprite = GetNode<AnimatedSprite>(_animatedSpritePath);
            _spawnAnimationPlayer = GetNode<AnimationPlayer>(_spawnAnimationPlayerPath);
        }

        public override void StateEntered()
        {
            _animatedSprite.FlipH = Main.RNG.RandiRange(0, 1) == 1;
            _parent.MetaSpawnPosition = _parent.GetOwnerOrNull<Node2D>().GlobalPosition;
        }

        public override void StateActive()
        {
            if (!_spawnAnimationPlayer.IsPlaying())
            {
                _parent.StateMachine.ChangeState(_wanderState);
            }
        }

        public override void StateLeft()
        {

        }
    }
}