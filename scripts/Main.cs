using Godot;

namespace GameFeel
{
    public class Main : Node
    {
        public static RandomNumberGenerator RNG { get; private set; } = new RandomNumberGenerator();

        private Viewport _gameViewport;
        private Node2D _inputTransformer;

        public override void _Ready()
        {
            RNG.Randomize();
            _gameViewport = GetNode<Viewport>("ViewportContainer/Viewport");
            _inputTransformer = _gameViewport.GetNode<Node2D>("InputTransformer");
        }

        public override void _Input(InputEvent evt)
        {
            _gameViewport.Input(_inputTransformer.MakeInputLocal(evt));
        }

        public override void _UnhandledInput(InputEvent evt)
        {
            _gameViewport.UnhandledInput(_inputTransformer.MakeInputLocal(evt));
        }
    }
}