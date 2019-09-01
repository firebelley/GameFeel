using Godot;

namespace GameFeel.Component
{
    public class QuestTriggerSpawnComponent : QuestTriggerComponent
    {
        [Export]
        private PackedScene _scene;

        protected override void Trigger()
        {
            if (_scene == null)
            {
                return;
            }

            var node = _scene.Instance() as Node2D;
            GameZone.EntitiesLayer.AddChild(node);
            node.GlobalPosition = GlobalPosition;
        }
    }
}