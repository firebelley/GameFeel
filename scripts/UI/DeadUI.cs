using GameFeel.Resource;
using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;

namespace GameFeel.UI
{
    public class DeadUI : Control
    {
        private const string ANIM_BOUNCE_IN = "ControlBounceIn";

        [Export]
        private NodePath _panelContainerPath;
        [Export]
        private NodePath _animationPlayerPath;
        [Export]
        private NodePath _respawnButtonPath;

        private PanelContainer _panelContainer;
        private AnimationPlayer _animationPlayer;
        private Button _respawnButton;

        public override void _Ready()
        {
            this.SetNodesByDeclaredNodePaths();
            _respawnButton.Connect("pressed", this, nameof(OnRespawnButtonPressed));
            _panelContainer.Connect("resized", this, nameof(OnPanelResized));
            GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventPlayerDied), this, nameof(OnPlayerDied));
            Hide();
        }

        private void OnPanelResized()
        {
            _panelContainer.RectPivotOffset = _panelContainer.RectSize / 2f;
        }

        private void OnPlayerDied()
        {
            Show();
            _animationPlayer.Play(ANIM_BOUNCE_IN);
        }

        private void OnRespawnButtonPressed()
        {
            Hide();
            ZoneTransitioner.TransitionToGraveyard();
        }
    }
}