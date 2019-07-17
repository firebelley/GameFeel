using Godot;

namespace GameFeel
{
    public class Main : Node
    {
        public static RandomNumberGenerator RNG { get; private set; } = new RandomNumberGenerator();

        public override void _Ready()
        {
            RNG.Randomize();
        }
    }
}