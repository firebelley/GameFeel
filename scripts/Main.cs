using Godot;

namespace GameFeel
{
    public class Main : Node
    {
        public const float UI_TO_GAME_DISPLAY_RATIO = .5f;
        public static RandomNumberGenerator RNG { get; private set; } = new RandomNumberGenerator();

        private Viewport _gameViewport;

        public override void _Ready()
        {
            RNG.Randomize();
            _gameViewport = GetNode<Viewport>("ViewportContainer/Viewport");
        }

        public override void _Input(InputEvent evt)
        {
            _gameViewport.Input(TransformInput(evt));
        }

        public override void _UnhandledInput(InputEvent evt)
        {
            _gameViewport.UnhandledInput(TransformInput(evt));
        }

        private InputEvent TransformInput(InputEvent evt)
        {
            if (evt is InputEventMouse e)
            {
                e.Position *= UI_TO_GAME_DISPLAY_RATIO;
                return e;
            }
            return evt;
        }
    }
}