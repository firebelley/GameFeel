using Godot;
using GodotTools.Extension;

namespace GameFeel.Component.Subcomponent.Behavior
{
    public class PlayAnimation : BehaviorNode
    {
        [Export(PropertyHint.Enum, "idle,run,attack")]
        private string _animationName;
        [Export]
        private bool _waitAnimationFinish = false;

        protected override void InternalEnter()
        {
            _root.Blackboard.AnimatedSprite.Play(_animationName);
            if (!_waitAnimationFinish)
            {
                Leave(Status.SUCCESS);
            }
            else
            {
                _root.Blackboard.AnimatedSprite.Connect("animation_finished", this, nameof(OnAnimationFinished));
            }
        }

        protected override void InternalLeave()
        {
            this.DisconnectAllSignals(_root.Blackboard.AnimatedSprite);
        }

        protected override void Tick()
        {

        }

        private void OnAnimationFinished()
        {
            if (!_waitAnimationFinish) return;

            if (_root.Blackboard.AnimatedSprite.Animation == _animationName)
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