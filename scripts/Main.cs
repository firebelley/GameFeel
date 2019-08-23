using Godot;

namespace GameFeel
{
    public class Main : Node
    {
        public const float UI_TO_GAME_DISPLAY_RATIO = .5f;
        public static RandomNumberGenerator RNG { get; private set; } = new RandomNumberGenerator();
        private static Main _instance;

        private Viewport _gameViewport;

        public override void _Ready()
        {
            _instance = this;
            RNG.Randomize();
            _gameViewport = GetNode<Viewport>("ViewportContainer/Viewport");
        }

        public override void _UnhandledInput(InputEvent evt)
        {
            _gameViewport.UnhandledInput(TransformInput(evt));
        }

        public static void SendInput(InputEvent evt)
        {
            _instance._gameViewport.UnhandledInput(_instance.TransformInput(evt));
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