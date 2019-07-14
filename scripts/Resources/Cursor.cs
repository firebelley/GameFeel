using Godot;

namespace GameFeel.Resources
{
    public class Cursor : Sprite
    {
        public override void _Ready()
        {
            Input.SetMouseMode(Input.MouseMode.Hidden);
        }

        public override void _Process(float delta)
        {
            GlobalPosition = GetGlobalMousePosition();
        }
    }
}