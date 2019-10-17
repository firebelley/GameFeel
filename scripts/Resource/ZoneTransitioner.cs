using System.Linq;
using GameFeel.GameObject;
using GameFeel.Singleton;
using Godot;
using GodotApiTools.Extension;
using GodotApiTools.Util;

namespace GameFeel.Resource
{
    public class ZoneTransitioner : Node
    {
        [Export]
        private string _debugStartZoneId;
        [Export]
        private NodePath _viewportPath;

        private static ZoneTransitioner _instance;
        private Viewport _viewport;
        private int _zoneTransitionAreaIndex = -1;

        public override void _Ready()
        {
            _instance = this;
            this.SetNodesByDeclaredNodePaths();

            if (OS.IsDebugBuild() && !string.IsNullOrEmpty(_debugStartZoneId))
            {
                Transition(MetadataLoader.ZoneIdToMetadata[_debugStartZoneId]);
            }
            else
            {
                Transition(MetadataLoader.ZoneIdToMetadata["ae44e7c6-923b-5b9b-a88a-c15cc41f5b56"]);
            }
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
                child.GetParent().RemoveChild(child);
                child.QueueFree();
            }

            _instance.CallDeferred(nameof(SwapZone), toZoneData.ResourcePath);
        }

        public static void TransitionToArea(MetadataLoader.Metadata toZoneData, int zoneTransitionAreaIndex)
        {
            _instance._zoneTransitionAreaIndex = zoneTransitionAreaIndex;
            Transition(toZoneData);
        }

        public static void TransitionToGraveyard()
        {
            Transition(MetadataLoader.ZoneIdToMetadata[MetadataLoader.GRAVEYARD_ZONE_ID]);
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
            GameEventDispatcher.DispatchZoneChangedEvent(zone.Id);
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
            GameZone.Camera.GlobalPosition = position;
            _zoneTransitionAreaIndex = -1;
        }

        private Player CreatePlayer()
        {
            var playerData = MetadataLoader.EntityIdToMetadata[MetadataLoader.PLAYER_ID];
            var playerScene = GD.Load(playerData.ResourcePath) as PackedScene;
            var player = playerScene.Instance() as Player;
            return player;
        }
    }
}