using Godot;

namespace GameFeel.UI
{
    public class DialogueOptionButton : Button
    {
        private string ANIM_BREATHE = "breathe";

        [Export]
        private Font _hoverFont;

        private Tween _tween;
        private ColorRect _colorRect;
        private Font _normalFont;
        private AnimationPlayer _animationPlayer;

        public override void _Ready()
        {
            _tween = GetNode<Tween>("Tween");
            _colorRect = GetNode<ColorRect>("ColorRect");
            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _normalFont = GetFont("font");

            CenterPivot();

            Connect("mouse_entered", this, nameof(OnMouseEntered));
            Connect("mouse_exited", this, nameof(OnMouseExited));
            Connect("resized", this, nameof(OnResized));
        }

        public void OffsetAnimation()
        {
            _animationPlayer.Seek(_animationPlayer.GetAnimation(ANIM_BREATHE).Length / 2f);
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

        private void CenterPivot()
        {
            RectPivotOffset = RectSize / 2f;
        }

        private void OnMouseEntered()
        {
            AddFontOverride("font", _hoverFont);
            PlayShaderEffect(false);
        }

        private void OnMouseExited()
        {
            AddFontOverride("font", _normalFont);
            PlayShaderEffect(true);
        }

        private void OnResized()
        {
            CenterPivot();
        }
    }
}