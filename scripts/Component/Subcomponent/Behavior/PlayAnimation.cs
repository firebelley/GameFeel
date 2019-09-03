using Godot;

namespace GameFeel.Component.Subcomponent.Behavior
{
    public class PlayAnimation : BehaviorNode
    {
        [Export]
        private NodePath _animatedSpritePath;
        [Export]
        private string _animationName;

        private AnimatedSprite _animatedSprite;

        public override void _Ready()
        {
            _animatedSprite = GetNodeOrNull<AnimatedSprite>(_animatedSpritePath ?? string.Empty);
        }

        protected override void InternalEnter()
        {
            _animatedSprite.Play(_animationName);
            Leave(Status.SUCCESS);
        }

        protected override void Tick()
        {

        }
    }
}