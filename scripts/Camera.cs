using Godot;
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

        private float _xNoiseSample;
        private float _yNoiseSample;

        private Vector2 _originalOffset;
        private Vector2 _offset;

        private static float _amplitude;

        public override void _Ready()
        {
            _originalOffset = Offset;
        }

        public override void _Process(float delta)
        {
            if (_amplitude > 0f)
            {
                _xNoiseSample += X_NOISE_GROWTH * delta;
                _yNoiseSample += Y_NOISE_GROWTH * delta;

                _xNoiseSample = Mathf.Wrap(_xNoiseSample, 0f, NOISE_MAX);
                _yNoiseSample = Mathf.Wrap(_yNoiseSample, 0f, NOISE_MAX);

                _offset.x = PerlinNoise.Noise(_xNoiseSample);
                _offset.y = PerlinNoise.Noise(_yNoiseSample);

                _amplitude = Mathf.Clamp(_amplitude - AMPLITUDE_DECAY * delta, 0f, 1f);

                _offset *= MAX_OFFSET * _amplitude * _amplitude;

                Offset = _originalOffset + _offset;
            }
        }

        public static void Shake()
        {
            _amplitude = 1f;
        }
    }
}