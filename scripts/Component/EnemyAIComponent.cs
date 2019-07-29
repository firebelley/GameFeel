using Godot;
using GodotTools.Logic;
using GodotTools.Logic.Interface;

namespace GameFeel.Component
{
    public class EnemyAIComponent : Node
    {
        // TODO: ASK FOR REQUIRED NODES HERE
        // REFERENCE REQUIRED NODES IN CHILD NODES VIA PROPERTIES
        public StateExecutorMachine StateMachine { get; private set; } = new StateExecutorMachine();

        public override void _Ready()
        {
            var first = GetChildren() [0] as IStateExector;
            StateMachine.ChangeState(first);
        }

        public override void _Process(float delta)
        {
            StateMachine.Update();
        }
    }
}