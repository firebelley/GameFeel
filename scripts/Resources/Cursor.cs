using Godot;

namespace GameFeel.Resources
{
    public class Cursor : Sprite
    {
        private const string ANIM_DEFAULT = "default";

        private AnimationPlayer _animationPlayer;

        public override void _Ready()
        {
            Input.SetMouseMode(Input.MouseMode.Hidden);
            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        }

        public override void _Process(float delta)
        {
            GlobalPosition = GetGlobalMousePosition();
            if (Input.IsActionJustPressed("attack"))
            {
                _animationPlayer.Stop();
                _animationPlayer.Play(ANIM_DEFAULT);
            }
        }
    }
}