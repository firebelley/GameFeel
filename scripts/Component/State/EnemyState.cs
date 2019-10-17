using Godot;
using GodotApiTools.Logic.Interface;

namespace GameFeel.Component.State
{
    public class EnemyState : Node, IStateExector
    {
        protected EnemyAIComponent _parent { get; private set; }
        protected Node2D _parentOwner { get; private set; }

        public override void _Ready()
        {
            _parent = GetParent() as EnemyAIComponent;
            _parentOwner = _parent.Owner as Node2D;
        }

        public virtual void StateActive()
        {

        }

        public virtual void StateEntered()
        {

        }

        public virtual void StateLeft()
        {

        }
    }
}