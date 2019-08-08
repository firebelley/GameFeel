using GameFeel.GameObject;
using Godot;
using GodotTools.Extension;
using GodotTools.Util;

namespace GameFeel
{
    public class Camera : Camera2D
    {
        private const float X_NOISE_GROWTH = 15f;
        private const float Y_NOISE_GROWTH = 14f;
        private const float NOISE_MAX = 5000f;
        private const float AMPLITUDE_DECAY = 6f;
        private const float MAX_OFFSET = 4f;
        private const float CAMERA_FOLLOW = 10f;

        private static float _amplitude;
        private static Camera _camera;
        // TODO: THIS IS A BAD SOLUTION
        public static Vector2 Shift;

        private float _xNoiseSample;
        private float _yNoiseSample;

        private Vector2 _originalOffset;
        private Vector2 _offset;
        private Vector2 _targetPosition;

        private ColorRect _colorRect;
        private Tween _tween;

        public override void _Ready()
        {
            _originalOffset = Offset;
            _colorRect = GetNode<ColorRect>("HitEffectLayer/ColorRect");
            _tween = GetNode<Tween>("HitEffectLayer/Tween");
            _camera = this;
            VisualServer.SetDefaultClearColor(new Color(15f / 255f, 15f / 255f, 15f / 255f, 1f));
        }

        public override void _Process(float delta)
        {
            var player = GetTree().GetFirstNodeInGroup<Player>(Player.GROUP);
            if (player != null)
            {
                _targetPosition = player.GetCameraTargetPosition() + Shift;
            }

            GlobalPosition = GlobalPosition.LinearInterpolate(_targetPosition, CAMERA_FOLLOW * delta);

            if (_amplitude > 0f)
            {
                _xNoiseSample += X_NOISE_GROWTH * delta;
                _yNoiseSample += Y_NOISE_GROWTH * delta;

                _xNoiseSample = Mathf.Wrap(_xNoiseSample, 0f, NOISE_MAX);
                _yNoiseSample = Mathf.Wrap(_yNoiseSample, 0f, NOISE_MAX);

                _offset.x = PerlinNoise.Noise(_xNoiseSample);
                _offset.y = PerlinNoise.Noise(_yNoiseSample);

                _amplitude = Mathf.Clamp(_amplitude - AMPLITUDE_DECAY * delta, 0f, 5f);

                _offset *= MAX_OFFSET * _amplitude * _amplitude;

                Offset = _originalOffset + _offset;
            }
        }

        public static void Flash()
        {
            var x = Main.RNG.RandfRange(-.1f, .1f);
            var y = Main.RNG.RandfRange(-.1f, .1f);
            var uvShift = new Vector2(x, y);

            uvShift.x += Main.RNG.RandiRange(0, 1);
            uvShift.y += Main.RNG.RandiRange(0, 1);

            _camera._colorRect.Material.Set("shader_param/_uvShift", uvShift);

            _camera._tween.ResetAll();
            _camera._tween.InterpolateProperty(
                _camera._colorRect.Material,
                "shader_param/_cutoff",
                0f,
                1f,
                .75f,
                Tween.TransitionType.Quad,
                Tween.EaseType.In
            );
            _camera._tween.Start();
        }

        public static void Shake(float magnitude)
        {
            _amplitude = magnitude;
        }
    }
}