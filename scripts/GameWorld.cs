using GameFeel.Effect;
using Godot;
using GodotTools.Extension;

namespace GameFeel
{
    public class GameWorld : Node
    {
        public static GameWorld Instance { get; private set; }

        public static YSort EntitiesLayer { get; private set; }
        public static YSort EffectsLayer { get; private set; }

        private Node _damageNumbersLayer;
        private ResourcePreloader _resourcePreloader;
        private Navigation2D _navigation;

        public override void _Ready()
        {
            Instance = this;
            EntitiesLayer = GetNode<YSort>("Entities");
            EffectsLayer = GetNode<YSort>("Effects");
            _damageNumbersLayer = GetNode<Node>("DamageNumbers");
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
            _navigation = GetNode<Navigation2D>("Navigation2D");
        }

        public static void CreateDamageNumber(Node2D sourceNode, float damage)
        {
            if (IsInstanceValid(Instance))
            {
                var damageNumber = Instance._resourcePreloader.InstanceScene<DamageNumber>();
                Instance._damageNumbersLayer.AddChild(damageNumber);
                damageNumber.SetNumber(damage);
                damageNumber.GlobalPosition = sourceNode.GlobalPosition;
            }
        }

        public static Curve2D GetPathCurve(Vector2 start, Vector2 end)
        {
            var curve = new Curve2D();
            if (!IsInstanceValid(Instance))
            {
                return curve;
            }
            var points = Instance._navigation.GetSimplePath(start, end, false);
            foreach (var point in points)
            {
                curve.AddPoint(point);
            }
            curve.AddPoint(end);
            return curve;
        }

        public static Vector2 GetViewportMousePosition()
        {
            if (!IsInstanceValid(Instance))
            {
                return Vector2.Zero;
            }

            if (Instance.GetParent() is Viewport vp)
            {
                return vp.GetCanvasTransform().XformInv(vp.GetMousePosition());
            }

            return Vector2.Zero;
        }
    }
}