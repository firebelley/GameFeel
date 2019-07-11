using Godot;

namespace GameFeel.Effect
{
    public class DamageNumber : Position2D
    {
        private const float SPEED = 150f;
        private Label _label;

        private RandomNumberGenerator _rng;
        private Vector2 _velocity;

        public override void _Ready()
        {
            _label = GetNode<Label>("Label");
            _rng = new RandomNumberGenerator();
            _rng.Randomize();
            var angle = _rng.RandfRange(90f - 15f, 90f + 15f);
            _velocity = -Vector2.Right.Rotated(Mathf.Deg2Rad(angle)) * SPEED;
        }

        public override void _Process(float delta)
        {
            _velocity.y += 300f * delta;
            GlobalPosition += _velocity * delta;
        }

        public void SetNumber(int number)
        {
            _label.Text = $"{number}";
        }

        public void SetNumber(float number)
        {
            SetNumber((int) number);
        }
    }
}