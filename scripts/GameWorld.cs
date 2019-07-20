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

        public static Curve2D GetPathCurve(Vector2 start, Vector2 end, float handleMagnitude)
        {
            var curve = new Curve2D();
            if (!IsInstanceValid(Instance))
            {
                return curve;
            }
            var points = Instance._navigation.GetSimplePath(start, end, false);
            for (int i = 0; i < points.Length; i++)
            {
                var point = points[i];

                var inVec = Vector2.Zero;
                var outVec = Vector2.Zero;

                if (i > 0)
                {
                    // var dir = point - points[i - 1];
                    // if (Mathf.Abs(dir.x) > 0.1f)
                    // {
                    //     inVec.y = Mathf.Sign(dir.x);
                    //     outVec.y = -inVec.y;
                    // }

                    // if (Mathf.Abs(dir.y) > 0.1f)
                    // {
                    //     inVec.x = -Mathf.Sign(dir.y);
                    //     outVec.x = -inVec.x;
                    // }
                    curve.AddPoint(point, inVec * handleMagnitude, outVec * handleMagnitude);
                }
                else
                {
                    curve.AddPoint(point);
                }

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