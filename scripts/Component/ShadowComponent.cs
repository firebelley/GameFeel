using Godot;

namespace GameFeel.Component
{
    [Tool]
    public class ShadowComponent : Node2D
    {
        [Export]
        private float _radius
        {
            get
            {
                return _realRadius;
            }
            set
            {
                _realRadius = value;
                Update();
            }
        }

        private float _realRadius;

        private Color _color = new Color(0f, 0f, 0f, 100f / 255f);

        public override void _Ready()
        {

        }

        public override void _Draw()
        {
            DrawCircle(Vector2.Zero, _radius, _color);
        }
    }

}