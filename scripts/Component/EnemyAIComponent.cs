using Godot;
using GodotApiTools.Extension;
using GodotApiTools.Logic;
using GodotApiTools.Logic.Interface;
using GodotApiTools.Util;

namespace GameFeel.Component
{
    public class EnemyAIComponent : Node
    {
        public const string META_ANIM_ATTACK = "attack";
        public const string META_ANIM_IDLE = "idle";
        public const string META_ANIM_RUN = "run";

        public StateExecutorMachine StateMachine { get; private set; } = new StateExecutorMachine();

        public Vector2 MetaSpawnPosition { get; set; }
        public PathfindComponent MetaPathfindComponent { get; private set; }
        public EntityDataComponent MetaEntityDataComponent { get; private set; }

        [Export]
        private NodePath _pathfindComponentPath;

        public override void _Ready()
        {
            MetaPathfindComponent = GetNodeOrNull<PathfindComponent>(_pathfindComponentPath ?? string.Empty);
            MetaEntityDataComponent = Owner.GetFirstNodeOfType<EntityDataComponent>();
            if (MetaPathfindComponent == null)
            {
                Logger.Error("No pathfind component set in " + Owner.Filename);
                QueueFree();
                return;
            }

            if (GetChildCount() > 0)
            {
                var first = GetChildren() [0] as IStateExector;
                StateMachine.ChangeState(first);
            }
        }

        public override void _Process(float delta)
        {
            StateMachine.Update();
        }
    }
}