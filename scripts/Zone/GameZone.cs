using System.Collections.Generic;
using GameFeel.Effect;
using Godot;
using GodotTools.Extension;

namespace GameFeel
{
    public class GameZone : Node
    {
        public static GameZone Instance { get; private set; }

        public static YSort EntitiesLayer { get; private set; }
        public static YSort EffectsLayer { get; private set; }

        [Export]
        private bool _drawNavigation;

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

            UpdateNavigationMesh();
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

        private void UpdateNavigationMesh()
        {

            var worldTileMap = GetNode<TileMap>("WorldTileMap");
            var worldTileSet = worldTileMap.TileSet;
            var blockedSet = new HashSet<Vector2>();

            foreach (var node in EntitiesLayer.GetChildren())
            {
                if (node is StaticBody2D staticBody)
                {
                    var collisionShape = staticBody.GetFirstNodeOfType<CollisionPolygon2D>();
                    if (collisionShape == null)
                    {
                        continue;
                    }

                    var centroid = Vector2.Zero;
                    var polygonPointsSum = Vector2.Zero;
                    foreach (var point in collisionShape.Polygon)
                    {
                        polygonPointsSum += point;
                    }
                    centroid = polygonPointsSum / collisionShape.Polygon.Length;

                    var scaledPolygonPoints = new List<Vector2>();
                    foreach (var point in collisionShape.Polygon)
                    {
                        // normalize the polygon
                        var newPoint = point - centroid;
                        newPoint *= .99f;
                        scaledPolygonPoints.Add(newPoint + centroid);
                    }

                    foreach (var point in scaledPolygonPoints)
                    {
                        GD.Print(point);
                        var blockedCellV = worldTileMap.WorldToMap(staticBody.GlobalPosition + point);
                        blockedSet.Add(blockedCellV);
                    }

                }
            }

            foreach (var cellvObj in worldTileMap.GetUsedCells())
            {
                var cellv = (Vector2) cellvObj;
                var cellId = worldTileMap.GetCellv(cellv);

                if (blockedSet.Contains(cellv))
                {
                    continue;
                }

                var poly = worldTileSet.AutotileGetNavigationPolygon(cellId, Vector2.Zero);
                if (poly == null)
                {
                    poly = worldTileSet.TileGetNavigationPolygon(cellId);
                    if (poly == null)
                    {
                        continue;
                    }
                }

                var transform = Transform2D.Identity;
                transform.origin = worldTileMap.MapToWorld(cellv);

                if (OS.IsDebugBuild() && _drawNavigation)
                {
                    var polygonNode = new Polygon2D();
                    polygonNode.Polygon = poly.GetVertices();
                    polygonNode.Transform = transform;
                    polygonNode.Modulate = new Color(0f, .5f, .5f, .5f);
                    _navigation.AddChild(polygonNode);
                }
                _navigation.NavpolyAdd(poly, transform);
            }
        }
    }
}