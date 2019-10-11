using System.Collections.Generic;
using GameFeel.GameObject.Effect;
using GameFeel.GameObject.Environment;
using Godot;
using GodotTools.Extension;

namespace GameFeel
{
    public class GameZone : Node
    {
        private const int TILE_SIZE = 16;

        public static GameZone Instance { get; private set; }

        public static YSort EntitiesLayer { get; private set; }
        public static YSort EffectsLayer { get; private set; }
        public static Node FloatersLayer { get; private set; }
        public static Camera2D Camera { get; private set; }

        [Export]
        public string Id { get; private set; }

        [Export]
        public string DisplayName { get; private set; }

        [Export]
        private bool _drawNavigation;

        public Vector2 DefaultPlayerSpawnPosition
        {
            get
            {
                return GetNode<Position2D>("DefaultPlayerSpawnPosition").GlobalPosition;
            }
        }

        public List<ZoneTransitionArea> ZoneTransitionAreas
        {
            get
            {
                return GetNode("ZoneTransitionAreas").GetChildren<ZoneTransitionArea>();
            }
        }

        private ResourcePreloader _resourcePreloader;
        private Navigation2D _navigation;

        public override void _Ready()
        {
            Instance = this;

            EntitiesLayer = GetNode<YSort>("Entities");
            EffectsLayer = GetNode<YSort>("Effects");
            FloatersLayer = GetNode<Node>("Floaters");
            Camera = GetNode<Camera2D>("Camera2D");
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
            _navigation = GetNode<Navigation2D>("Navigation2D");

            UpdateNavigationMesh();
        }

        public static void CreateDamageNumber(Node2D sourceNode, float damage)
        {
            if (IsInstanceValid(Instance))
            {
                var damageNumber = Instance._resourcePreloader.InstanceScene<DamageNumber>();
                FloatersLayer.AddChild(damageNumber);
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

                curve.AddPoint(point, inVec * handleMagnitude, outVec * handleMagnitude);

            }
            curve.AddPoint(end);
            return curve;
        }

        public static Curve2D GetStraightCurve(Vector2 fromPos, Vector2 toPos)
        {
            var curve = new Curve2D();
            var raycast = Instance.GetViewport().GetWorld2d().DirectSpaceState.Raycast(fromPos, toPos, null, 1);
            var endPos = raycast?.Position ?? toPos;
            curve.AddPoint(fromPos);
            curve.AddPoint(endPos);
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
                    var collisionShapeNode = staticBody.GetFirstNodeOfType<CollisionShape2D>();
                    if (collisionShapeNode == null)
                    {
                        continue;
                    }

                    var collisionShape = collisionShapeNode.Shape as RectangleShape2D;
                    if (collisionShape == null)
                    {
                        continue;
                    }

                    List<Vector2> subdividedPoints = new List<Vector2>();
                    var shapeSize = collisionShape.Extents * 2f;
                    var xOffset = -collisionShape.Extents.x;
                    while (xOffset < collisionShape.Extents.x)
                    {
                        var yOffset = -collisionShape.Extents.y;
                        while (yOffset < collisionShape.Extents.y)
                        {
                            subdividedPoints.Add(new Vector2(xOffset, yOffset));
                            yOffset += TILE_SIZE;
                        }
                        xOffset += TILE_SIZE;
                    }

                    // add the right corners
                    subdividedPoints.Add(new Vector2(collisionShape.Extents.x, -collisionShape.Extents.y));
                    subdividedPoints.Add(new Vector2(collisionShape.Extents.x, collisionShape.Extents.y));

                    foreach (var point in subdividedPoints)
                    {
                        var blockedCellV = worldTileMap.WorldToMap(collisionShapeNode.GlobalPosition + point * .99f);
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

                var cellCoord = worldTileMap.GetCellAutotileCoord((int) cellv.x, (int) cellv.y);
                var poly = worldTileSet.AutotileGetNavigationPolygon(cellId, cellCoord);
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