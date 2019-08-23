using GameFeel.GameObject;
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
        }

        public override void _Process(float delta)
        {
            var player = GetTree().GetFirstNodeInGroup<Player>(Player.GROUP);
            if (player == null)
            {
                return;
            }
            var prevManaValue = _manaBar.Value;
            _manaBar.Value = player.Mana / (player.MaxMana > 0f ? player.MaxMana : 1f);
            _manaLabel.Text = string.Format(RESOURCE_LABEL_FORMAT, Mathf.Floor(player.Mana), player.MaxMana);

            if (prevManaValue > _manaBar.Value + RESOURCE_ANIM_THRESHHOLD)
            {
                _manaBarAnimationPlayer.Stop();
                _manaBarAnimationPlayer.Play(ANIM_DEFAULT);
            }

            var prevHealthValue = _healthBar.Value;
            _healthBar.Value = player.Health / (player.MaxHealth > 0f ? player.MaxHealth : 1f);
            _healthLabel.Text = string.Format(RESOURCE_LABEL_FORMAT, Mathf.Floor(player.Health), player.MaxHealth);

            if (prevHealthValue > _healthBar.Value + RESOURCE_ANIM_THRESHHOLD)
            {
                _healthBarAnimationPlayer.Stop();
                _healthBarAnimationPlayer.Play(ANIM_DEFAULT);
            }
        }
    }
}