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
            _cursorRoot = GetNode<Node2D>("Scaler/Cursor");
            _primary = GetNode<Sprite>("Scaler/Cursor/PrimarySprite");
            _secondary = GetNode<Sprite>("Scaler/Cursor/SecondarySprite");
            _animationPlayer = GetNode<AnimationPlayer>("Scaler/Cursor/AnimationPlayer");
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
            _instance._secondary.Modulate = new Color(1f, 1f, 1f, .75f);
            _instance._primary.Visible = texture == null;
        }

        public static Vector2 GetAdjustedGlobalMousePosition(Node2D n)
        {
            var mousePos = n.GetTree().GetRoot().GetMousePosition() * .5f;
            return n.GetCanvasTransform().AffineInverse().Xform(mousePos);
        }
    }
}