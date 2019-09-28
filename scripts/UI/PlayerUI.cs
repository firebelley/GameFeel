using GameFeel.GameObject;
using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;

namespace GameFeel.UI
{
    public class PlayerUI : Control
    {
        private const string ANIM_DEFAULT = "default";
        private const float RESOURCE_ANIM_THRESHHOLD = .01f;
        private const string RESOURCE_LABEL_FORMAT = "{0:0}/{1:0}";

        [Export]
        private NodePath _manaBarPath;
        [Export]
        private NodePath _manaLabelPath;
        [Export]
        private NodePath _manaBarAnimationPlayerPath;

        [Export]
        private NodePath _healthBarPath;
        [Export]
        private NodePath _healthLabelPath;
        [Export]
        private NodePath _healthBarAnimationPlayerPath;

        private ProgressBar _manaBar;
        private AnimationPlayer _manaBarAnimationPlayer;
        private AnimationPlayer _healthBarAnimationPlayer;
        private Label _manaLabel;
        private ProgressBar _healthBar;
        private Label _healthLabel;

        public override void _Ready()
        {
            this.SetNodesByDeclaredNodePaths();
            GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventPlayerHealthChanged), this, nameof(OnPlayerHealthChanged));
            GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventPlayerManaChanged), this, nameof(OnPlayerManaChanged));
            GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventPlayerCreated), this, nameof(OnPlayerCreated));
        }

        private void OnPlayerHealthChanged(string eventGuid, Player player)
        {
            UpdateHealth(player);
        }

        private void OnPlayerManaChanged(string eventGuid, Player player)
        {
            UpdateMana(player);
        }

        private void UpdateProgressBar(ProgressBar bar, AnimationPlayer animationPlayer, Label label, float currentResource, float maxResource)
        {
            var prevHealthValue = bar.Value;
            bar.Value = currentResource / (maxResource > 0f ? maxResource : 1f);
            label.Text = string.Format(RESOURCE_LABEL_FORMAT, Mathf.Floor(currentResource), maxResource);

            if (prevHealthValue > bar.Value + RESOURCE_ANIM_THRESHHOLD)
            {
                animationPlayer.Stop();
                animationPlayer.Play(ANIM_DEFAULT);
            }
        }

        private void UpdateHealth(Player player)
        {
            UpdateProgressBar(_healthBar, _healthBarAnimationPlayer, _healthLabel, player.Health, player.MaxHealth);
        }

        private void UpdateMana(Player player)
        {
            UpdateProgressBar(_manaBar, _manaBarAnimationPlayer, _manaLabel, player.Mana, player.MaxMana);
        }

        private void OnPlayerCreated(string eventGuid, Player player)
        {
            UpdateHealth(player);
            UpdateMana(player);
        }
    }
}