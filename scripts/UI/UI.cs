using GameFeel.GameObject;
using Godot;
using GodotTools.Extension;

namespace GameFeel.UI
{
    public class UI : CanvasLayer
    {
        [Export]
        private NodePath _manaBarPath;

        private ProgressBar _manaBar;

        public override void _Ready()
        {
            this.SetNodesByDeclaredNodePaths();
        }

        public override void _Process(float delta)
        {
            var players = GetTree().GetNodesInGroup(Player.GROUP);
            if (players.Count == 0)
            {
                return;
            }

            var player = players[0] as Player;
            _manaBar.Value = player.Mana / (player.MaxMana > 0f ? player.MaxMana : 1f);
        }
    }
}