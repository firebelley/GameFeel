using GameFeel.Component;
using Godot;

namespace GameFeel.UI
{
    [Tool]
    public class HealthBarComponent : Node2D
    {
        [Export]
        private NodePath _healthComponentPath;

        private AnimationPlayer _animationPlayer;
        private ProgressBar _progressBar;

        public override void _Ready()
        {
            _progressBar = GetNode<ProgressBar>("ProgressBar");
            _animationPlayer = GetNode<AnimationPlayer>("ProgressBar/AnimationPlayer");

            GetHealthComponent()?.Connect(nameof(HealthComponent.HealthDecremented), this, nameof(OnHealthDecremented));
        }

        public override string _GetConfigurationWarning()
        {
            if (!IsInstanceValid(GetOwner()) || GetHealthComponent() == null)
            {
                return "Will not display properly without owner having a child " + nameof(HealthComponent);
            }
            return string.Empty;
        }

        private HealthComponent GetHealthComponent()
        {
            if (_healthComponentPath != null)
            {
                return GetNodeOrNull<HealthComponent>(_healthComponentPath);
            }
            return null;
        }

        private void OnHealthDecremented()
        {
            _progressBar.Value = GetHealthComponent()?.GetHealthPercentage() ?? 0f;
            _animationPlayer.Stop(true);
            _animationPlayer.Play("bounce");
        }
    }
}