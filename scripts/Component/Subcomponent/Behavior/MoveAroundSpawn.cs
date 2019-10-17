using Godot;
using GodotApiTools.Extension;

namespace GameFeel.Component.Subcomponent.Behavior
{
    public class MoveAroundSpawn : BehaviorNode
    {
        [Export]
        private float _radius = 50f;

        public override void _Ready()
        {
            base._Ready();
        }

        protected override void InternalEnter()
        {
            var offset = Vector2.Right.Rotated(Main.RNG.RandfRange(0f, 2f * Mathf.Pi)) * _radius;
            _root.Blackboard.PathfindComponent.UpdateStraightPath(_root.Blackboard.SpawnPosition, _root.Blackboard.SpawnPosition + offset);
            _root.Blackboard.PathfindComponent.Connect(nameof(PathfindComponent.PathEndReached), this, nameof(OnPathEndReached));
            SetProcess(true);
        }

        protected override void Tick()
        {
            if (_root.Blackboard.PathfindComponent.Velocity.x < -5f)
            {
                _root.Blackboard.AnimatedSprite.FlipH = true;
            }
            else if (_root.Blackboard.PathfindComponent.Velocity.x > 5f)
            {
                _root.Blackboard.AnimatedSprite.FlipH = false;
            }
        }

        protected override void InternalLeave()
        {
            _root.Blackboard.PathfindComponent.ClearPath();
            this.DisconnectAllSignals(_root.Blackboard.PathfindComponent);
        }

        private void OnPathEndReached()
        {
            Leave(Status.SUCCESS);
        }
    }
}