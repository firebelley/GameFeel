using System.Collections.Generic;
using Godot;
using GodotTools.Extension;

namespace GameFeel.Component
{
    [Tool]
    public class PathfindDrawComponent : Node2D
    {
        public override string _GetConfigurationWarning()
        {
            var pathfindComponent = GetOwner()?.GetFirstNodeOfType<PathfindComponent>();
            if (pathfindComponent == null)
            {
                return "Owner must contain a " + nameof(PathfindComponent);
            }
            return string.Empty;
        }

        public override void _Process(float delta)
        {
            Update();
        }

        public override void _Draw()
        {
            if (Engine.IsEditorHint())
            {
                return;
            }

            var points = GetOwner().GetFirstNodeOfType<PathfindComponent>().Curve.GetBakedPoints();
            List<Vector2> newPoints = new List<Vector2>();

            foreach (var point in points)
            {
                newPoints.Add(GetGlobalTransform().XformInv(point));
            }

            DrawPolyline(newPoints.ToArray(), new Color(1f, 1f, 1f), 1);
        }
    }
}