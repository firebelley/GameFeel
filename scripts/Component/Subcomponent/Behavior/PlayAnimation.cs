using Godot;
using GodotTools.Extension;

namespace GameFeel.Component.Subcomponent.Behavior
{
    public class PlayAnimation : BehaviorNode
    {
        [Export]
        private NodePath _animatedSpritePath;
        [Export]
        private string _animationName;
        [Export]
        private bool _waitAnimationFinish = false;

        private AnimatedSprite _animatedSprite;

        public override void _Ready()
        {
            base._Ready();
            _animatedSprite = GetNodeOrNull<AnimatedSprite>(_animatedSpritePath ?? string.Empty);
        }

        protected override void InternalEnter()
        {
            _animatedSprite.Play(_animationName);
            if (!_waitAnimationFinish)
            {
                Leave(Status.SUCCESS);
            }
            else
            {
                _animatedSprite.Connect("animation_finished", this, nameof(OnAnimationFinished));
            }
        }

        protected override void InternalLeave()
        {
            this.DisconnectAllSignals(_animatedSprite);
        }

        protected override void Tick()
        {

        }

        private void OnAnimationFinished()
        {
            if (!_waitAnimationFinish) return;

            if (_animatedSprite.Animation == _animationName)
            {
                Leave(Status.SUCCESS);
            }
            else
            {
                Leave(Status.FAIL);
            }
        }
    }
}