using GameFeel.Resource;
using GameFeel.Singleton;
using Godot;

namespace GameFeel.GameObject.Environment
{
    public class ZoneTransitionArea : Area2D
    {
        [Export(PropertyHint.Enum, "0,1,2,3,4")]
        private int _index = 0;
        [Export]
        public string ToZoneId { get; private set; }

        public override void _Ready()
        {
            Connect("body_entered", this, nameof(OnBodyEntered));
        }

        private void OnBodyEntered(PhysicsBody2D body)
        {
            var metaData = MetadataLoader.ZoneIdToMetadata[ToZoneId];
            ZoneTransitioner.TransitionToArea(metaData, _index);
        }
    }
}