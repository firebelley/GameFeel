using GameFeel.Effect;
using Godot;
using GodotTools.Extension;

namespace GameFeel
{
    public class Main : Node
    {
        public static Main Instance { get; private set; }

        private YSort _entitiesLayer;
        private Node _damageNumbersLayer;
        private YSort _effectsLayer;
        private ResourcePreloader _resourcePreloader;

        public override void _Ready()
        {
            Instance = this;
            _entitiesLayer = GetNode<YSort>("Entities");
            _effectsLayer = GetNode<YSort>("Effects");
            _damageNumbersLayer = GetNode<Node>("DamageNumbers");
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
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

        public static void CreateEffect(PackedScene packedScene, Vector2 globalPosition)
        {
            if (IsInstanceValid(Instance))
            {
                var scene = (Node2D) packedScene.Instance();
                Instance._effectsLayer.AddChild(scene);
                scene.GlobalPosition = globalPosition;
            }
        }
    }
}