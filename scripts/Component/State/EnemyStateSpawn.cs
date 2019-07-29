using Godot;
using GodotTools.Extension;
using GodotTools.Logic.Interface;

namespace GameFeel.Component.State
{
    public class EnemyStateSpawn : Node, IStateExector
    {
        [Signal]
        public delegate void SpawnPositionUpdated(Vector2 position);

        [Export]
        private NodePath _wanderStateNodePath;

        private EnemyAIComponent _parent;
        private IStateExector _wanderState;

        private AnimatedSprite _animatedSprite;
        private AnimationPlayer _animationPlayer;

        public override void _Ready()
        {
            _parent = GetParent() as EnemyAIComponent;
            if (_wanderStateNodePath != null)
            {
                _wanderState = GetNode(_wanderStateNodePath) as IStateExector;
            }
            _animatedSprite = _parent?.Owner?.GetFirstNodeOfType<AnimatedSprite>();
            _animationPlayer = _parent?.Owner?.GetFirstNodeOfType<AnimationPlayer>();
        }

        public void StateEntered()
        {
            _animatedSprite.FlipH = Main.RNG.RandiRange(0, 1) == 1;
            EmitSignal(nameof(SpawnPositionUpdated), _parent.GetOwnerOrNull<Node2D>().GlobalPosition);
        }

        public void StateActive()
        {
            if (!_animationPlayer.IsPlaying())
            {
                _parent.StateMachine.ChangeState(_wanderState);
            }
        }

        public void StateLeft()
        {

        }
    }
}