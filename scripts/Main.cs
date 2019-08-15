using Godot;

namespace GameFeel
{
    public class Main : Node
    {
        public static RandomNumberGenerator RNG { get; private set; } = new RandomNumberGenerator();

        public override void _Ready()
        {
            File file = new File();
            if (file.FileExists("res://test.quest"))
            {
                GetTree().Quit();
            }
            RNG.Randomize();
        }
    }
}