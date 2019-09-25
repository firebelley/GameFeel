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
            var zone = zoneScene.Instance();
            _viewport.AddChild(zone);

            // TODO: if zone transition area index > -1 then place the player at the indicated area index
            // if area index not found, then do default placement behavior

            _zoneTransitionAreaIndex = -1;
        }

        private void OnPlayerDied()
        {
            TransitionToArea(MetadataLoader.ZoneIdToMetadata[MetadataLoader.GRAVEYARD_ZONE_ID], -1);
        }
    }
}