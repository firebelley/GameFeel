using Godot;
using GodotTools.Extension;

namespace GameFeel.Component.Subcomponent.Behavior
{
    public class MoveAroundSpawn : BehaviorNode
    {
        [Export]
        private NodePath _pathfindComponentPath;
        [Export]
        private NodePath _animatedSpritePath;
        [Export]
        private float _radius = 50f;

        private PathfindComponent _pathfindComponent;
        private AnimatedSprite _animatedSprite;

        public override void _Ready()
        {
            base._Ready();
            _pathfindComponent = GetNodeOrNull<PathfindComponent>(_pathfindComponentPath ?? string.Empty);
            _animatedSprite = GetNodeOrNull<AnimatedSprite>(_animatedSpritePath ?? string.Empty);
        }

        protected override void InternalEnter()
        {
            var offset = Vector2.Right.Rotated(Main.RNG.RandfRange(0f, 2f * Mathf.Pi)) * _radius;
            _pathfindComponent.UpdateStraightPath(_root.Blackboard.SpawnPosition, _root.Blackboard.SpawnPosition + offset);
            _pathfindComponent.Connect(nameof(PathfindComponent.PathEndReached), this, nameof(OnPathEndReached));
            SetProcess(true);
        }

        protected override void Tick()
        {
            if (_pathfindComponent.Velocity.x < -5f)
            {
                _animatedSprite.FlipH = true;
            }
            else if (_pathfindComponent.Velocity.x > 5f)
            {
                _animatedSprite.FlipH = false;
            }
        }

        protected override void InternalLeave()
        {
            _pathfindComponent.ClearPath();
            this.DisconnectAllSignals(_pathfindComponent);
        }

        private void OnPathEndReached()
        {
            Leave(Status.SUCCESS);
        }
    }
}