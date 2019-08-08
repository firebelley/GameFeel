using GameFeel.Component.Subcomponent;
using Godot;
using GodotTools.Extension;
using GodotTools.Logic;

namespace GameFeel.Component
{
    [Tool]
    public class LootTableComponent : Node
    {
        [Export]
        private NodePath _healthComponentPath;

        private LootTable<LootTableItem> _lootTable = new LootTable<LootTableItem>();

        public override void _Ready()
        {
            _lootTable.SetRandom(Main.RNG);
            foreach (var child in GetChildren())
            {
                if (child is LootTableItem li)
                {
                    _lootTable.AddItem(li, li.Weight);
                }
            }

            if (_healthComponentPath != null)
            {
                GetNodeOrNull<HealthComponent>(_healthComponentPath)?.Connect(nameof(HealthComponent.HealthDepleted), this, nameof(OnHealthDepleted));
            }
        }

        public override string _GetConfigurationWarning()
        {
            var valid = true;
            foreach (var child in GetChildren())
            {
                if (!(child is LootTableItem))
                {
                    valid = false;
                    break;
                }
            }

            return valid ? string.Empty : "Must contain only children of type " + nameof(LootTableItem);
        }

        private void SpawnItem()
        {
            var toSpawn = _lootTable.PickItem();
            if (GetOwner() is Node2D node)
            {
                var scene = toSpawn.Scene?.Instance() as Node2D;
                if (scene != null)
                {
                    GameZone.EntitiesLayer.AddChildDeferred(scene);
                    scene.GlobalPosition = node.GlobalPosition;
                }
            }
        }

        private void OnHealthDepleted()
        {
            SpawnItem();
        }
    }
}