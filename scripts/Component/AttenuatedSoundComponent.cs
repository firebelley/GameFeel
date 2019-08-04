using Godot;

namespace GameFeel.Component
{
    public class AttenuatedSoundComponent : AudioStreamPlayer2D
    {
        private const float MIN_PITCH = .9f;
        private const float MAX_PITCH = 1.1f;

        [Export]
        private bool _randomPitch;
        [Export]
        private bool _randomSeek;
        [Export(PropertyHint.Range, "0,3")]
        private float _fadeDuration = .5f;
        [Export]
        private NodePath _projectileDeleterComponentPath;

        private Tween _tween;

        public override void _Ready()
        {
            if (_randomPitch)
            {
                PitchScale = Main.RNG.RandfRange(MIN_PITCH, MAX_PITCH);
            }

            if (_randomSeek && Stream != null)
            {
                var topos = Main.RNG.RandfRange(0f, Stream.GetLength());
                Seek(topos);
            }

            if (_projectileDeleterComponentPath != null)
            {
                GetNode<ProjectileDeleterComponent>(_projectileDeleterComponentPath).Connect(nameof(ProjectileDeleterComponent.Deleted), this, nameof(OnDeleted));
            }

            _tween = GetNode<Tween>("Tween");
            _tween.Connect("tween_all_completed", this, nameof(OnTweenCompleted));
        }

        private void OnDeleted()
        {
            _tween.ResetAll();
            _tween.InterpolateProperty(
                this,
                "volume_db",
                VolumeDb,
                VolumeDb - 20f,
                _fadeDuration,
                Tween.TransitionType.Linear,
                Tween.EaseType.In
            );
            _tween.Start();
        }

        private void OnTweenCompleted()
        {
            Playing = false;
        }
    }
}