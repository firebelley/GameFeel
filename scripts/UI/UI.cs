using GameFeel.GameObject;
using Godot;
using GodotTools.Extension;

namespace GameFeel.UI
{
    public class UI : CanvasLayer
    {
        private const string ANIM_DEFAULT = "default";
        private const float MANA_ANIM_THRESHOLD = .01f;
        private const string MANA_LABEL_FORMAT = "{0:0}/{1:0}";

        [Export]
        private NodePath _manaBarPath;
        [Export]
        private NodePath _manaLabelPath;
        [Export]
        private NodePath _manaBarAnimationPlayerPath;

        private ProgressBar _manaBar;
        private AnimationPlayer _manaBarAnimationPlayer;
        private Label _manaLabel;

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
            var prevValue = _manaBar.Value;
            _manaBar.Value = player.Mana / (player.MaxMana > 0f ? player.MaxMana : 1f);
            _manaLabel.Text = string.Format(MANA_LABEL_FORMAT, Mathf.Floor(player.Mana), player.MaxMana);

            if (prevValue > _manaBar.Value + MANA_ANIM_THRESHOLD)
            {
                _manaBarAnimationPlayer.Stop();
                _manaBarAnimationPlayer.Play(ANIM_DEFAULT);
            }
        }
    }
}