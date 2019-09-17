using GameFeel.Data;
using Godot;

namespace GameFeel.Resource
{
    public class Cursor : CanvasLayer
    {
        private const string ANIM_DEFAULT = "default";

        public static int DragIndex { get; private set; }
        public static DragSource DragFrom { get; private set; }
        public static InventoryItem Dragging
        {
            get
            {
                return _dragging;
            }
            private set
            {
                _dragging = value;
                SetSecondaryTexture(_dragging?.Icon ?? null);
            }
        }

        private static Cursor _instance;
        private static InventoryItem _dragging;

        private AnimationPlayer _animationPlayer;
        private Node2D _cursorRoot;
        private Sprite _primary;
        private Sprite _secondary;

        public enum DragSource
        {
            NONE,
            INVENTORY,
            EQUIPMENT
        }

        public override void _Ready()
        {
            _instance = this;
            Input.SetMouseMode(Input.MouseMode.Hidden);
            _cursorRoot = GetNode<Node2D>("Scaler/Cursor");
            _primary = GetNode<Sprite>("Scaler/Cursor/PrimarySprite");
            _secondary = GetNode<Sprite>("Scaler/Cursor/SecondarySprite");
            _animationPlayer = GetNode<AnimationPlayer>("Scaler/Cursor/AnimationPlayer");
            ClearDragSelection();
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

        public static void Drag(DragSource from, InventoryItem dragging, int dragIndex)
        {
            DragFrom = from;
            Dragging = dragging;
            DragIndex = dragIndex;
        }

        public static Vector2 GetUIPosition()
        {
            return _instance._cursorRoot.GlobalPosition;
        }

        public static Vector2 GetAdjustedGlobalMousePosition(Node2D n)
        {
            var mousePos = n.GetTree().GetRoot().GetMousePosition() * Main.UI_TO_GAME_DISPLAY_RATIO;
            return n.GetCanvasTransform().AffineInverse().Xform(mousePos);
        }

        public static void ClearDragSelection()
        {
            DragIndex = -1;
            DragFrom = DragSource.NONE;
            Dragging = null;
        }

        private static void SetSecondaryTexture(Texture texture)
        {
            _instance._secondary.Texture = texture;
            _instance._secondary.Modulate = new Color(1f, 1f, 1f, .75f);
            _instance._primary.Visible = texture == null;
        }
    }
}