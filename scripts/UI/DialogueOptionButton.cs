using Godot;

namespace GameFeel.UI
{
    public class DialogueOptionButton : Button
    {
        private Tween _tween;
        private ColorRect _colorRect;

        public override void _Ready()
        {
            _tween = GetNode<Tween>("Tween");
            _colorRect = GetNode<ColorRect>("ColorRect");
            Connect("mouse_entered", this, nameof(OnMouseEntered));
            Connect("mouse_exited", this, nameof(OnMouseExited));
        }

        private void PlayShaderEffect(bool backwards)
        {
            _tween.ResetAll();
            _tween.InterpolateProperty(
                _colorRect.Material,
                "shader_param/_cutoff",
                backwards ? 1 : 0,
                backwards ? 0 : 1,
                .15f,
                Tween.TransitionType.Linear,
                Tween.EaseType.In
            );
            _tween.Start();
        }

        private void OnMouseEntered()
        {
            GD.Print("yo");
            PlayShaderEffect(false);
        }

        private void OnMouseExited()
        {
            PlayShaderEffect(true);
        }
    }
}