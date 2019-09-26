using System.Linq;
using GameFeel.GameObject;
using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;
using GodotTools.Util;

namespace GameFeel.Resource
{
    public class ZoneTransitioner : Node
    {
        [Export]
        private NodePath _viewportPath;

        private static ZoneTransitioner _instance;
        private Viewport _viewport;
        private int _zoneTransitionAreaIndex;

        public override void _Ready()
        {
            _instance = this;
            this.SetNodesByDeclaredNodePaths();

            GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventPlayerDied), this, nameof(OnPlayerDied));

            Transition(MetadataLoader.ZoneIdToMetadata["267df08a-63fb-54da-9e30-ac39612ac708"]);
        }

        public static void Transition(MetadataLoader.Metadata toZoneData)
        {
            if (!IsInstanceValid(_instance))
            {
                Logger.Error("ZoneTransitioner instance was not valid.");
                return;
            }

            foreach (var child in _instance._viewport.GetChildren<Node>())
            {
                child.QueueFree();
            }

            _instance.CallDeferred(nameof(SwapZone), toZoneData.ResourcePath);
        }

        public static void TransitionToArea(MetadataLoader.Metadata toZoneData, int zoneTransitionAreaIndex)
        {
            _instance._zoneTransitionAreaIndex = zoneTransitionAreaIndex;
            Transition(toZoneData);
        }

        private void SwapZone(string zonePath)
        {
            var zoneScene = GD.Load(zonePath) as PackedScene;
            var zone = zoneScene.Instance() as GameZone;

            if (zone == null)
            {
                Logger.Error("Zone with path " + zonePath + " was not a " + nameof(GameZone));
                return;
            }
            _viewport.AddChild(zone);
            CreateAndPlacePlayer();
        }

        private void CreateAndPlacePlayer()
        {
            var transitionArea = GameZone.Instance.ZoneTransitionAreas.Where(x => x.Index == _zoneTransitionAreaIndex).FirstOrDefault();
            Vector2 position;
            if (transitionArea == null)
            {
                position = GameZone.Instance.DefaultPlayerSpawnPosition;
            }
            else
            {
                position = transitionArea.SpawnPosition;
            }

            var player = CreatePlayer();
            GameZone.EntitiesLayer.AddChild(player);
            player.GlobalPosition = position;
            _zoneTransitionAreaIndex = -1;
        }

        private Player CreatePlayer()
        {
            var playerData = MetadataLoader.EntityIdToMetadata[MetadataLoader.PLAYER_ID];
            var playerScene = GD.Load(playerData.ResourcePath) as PackedScene;
            var player = playerScene.Instance() as Player;
            return player;
        }

        private void OnPlayerDied()
        {
            TransitionToArea(MetadataLoader.ZoneIdToMetadata[MetadataLoader.GRAVEYARD_ZONE_ID], -1);
        }
    }
}