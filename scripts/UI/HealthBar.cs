using GameFeel.Component;
using GameFeel.GameObject;
using Godot;
using GodotTools.Extension;

namespace GameFeel.UI
{
    [Tool]
    public class HealthBar : Node2D
    {
        private AnimationPlayer _animationPlayer;
        private ProgressBar _progressBar;

        public override void _Ready()
        {
            _progressBar = GetNode<ProgressBar>("ProgressBar");
            _animationPlayer = GetNode<AnimationPlayer>("ProgressBar/AnimationPlayer");
            if (GetOwner() is Spider s)
            {
                s.Connect(nameof(Spider.DamageReceived), this, nameof(OnDamageReceived));
            }
        }

        public override string _GetConfigurationWarning()
        {
            if (!IsInstanceValid(GetOwner()) || GetOwner().GetFirstNodeOfType<HealthComponent>() == null)
            {
                return "Will not display properly without owner having a child " + nameof(HealthComponent);
            }
            return string.Empty;
        }

        private void OnDamageReceived(float damage)
        {
            var healthComponent = GetOwner().GetFirstNodeOfType<HealthComponent>();
            if (healthComponent == null)
            {
                return;
            }
            _progressBar.Value = healthComponent.GetHealthPercentage();
            _animationPlayer.Stop(true);
            _animationPlayer.Play("bounce");
        }
    }
}