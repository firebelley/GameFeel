using Godot;
using GodotTools.Logic;
using GodotTools.Logic.Interface;

namespace GameFeel.Component
{
    public class EnemyAIComponent : Node
    {
        public const string META_ANIM_ATTACK = "attack";
        public const string META_ANIM_IDLE = "idle";
        public const string META_ANIM_RUN = "run";

        public StateExecutorMachine StateMachine { get; private set; } = new StateExecutorMachine();

        public Vector2 MetaSpawnPosition { get; set; }

        public override void _Ready()
        {
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