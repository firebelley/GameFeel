using Godot;

namespace GameFeel.Resource
{
    public class Cursor : CanvasLayer
    {
        private const string ANIM_DEFAULT = "default";

        private static Cursor _instance;
        private AnimationPlayer _animationPlayer;
        private Node2D _cursorRoot;
        private Sprite _primary;
        private Sprite _secondary;

        public override void _Ready()
        {
            _instance = this;
            Input.SetMouseMode(Input.MouseMode.Hidden);
            _cursorRoot = GetNode<Node2D>("Cursor");
            _primary = GetNode<Sprite>("Cursor/PrimarySprite");
            _secondary = GetNode<Sprite>("Cursor/SecondarySprite");
            _animationPlayer = GetNode<AnimationPlayer>("Cursor/AnimationPlayer");
        }

        public override void _Process(float delta)
        {
            _cursorRoot.GlobalPosition = _cursorRoot.GetGlobalMousePosition();
            if (Input.IsActionJustPressed("attack"))
            {
                _animationPlayer.Stop();
                _animationPlayer.Play(ANIM_DEFAULT);
            }
        }

        public static void SetSecondaryTexture(Texture texture)
        {
            _instance._secondary.Texture = texture;
            _instance._primary.Visible = texture == null;
        }
    }
}