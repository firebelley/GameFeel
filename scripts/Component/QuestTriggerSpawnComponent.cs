using GameFeel.GameObject.Loot;
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

            if (node is LootItem li)
            {
                li.Persist = true;
            }

            GameZone.EntitiesLayer.AddChild(node);
            node.GlobalPosition = GlobalPosition;
        }
    }
}