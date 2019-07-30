using Godot;

namespace GameFeel.Component.Subcomponent
{
    [Tool]
    public class LootTableItem : Node
    {
        [Export]
        public int Weight { get; private set; }

        [Export]
        public PackedScene Scene { get; private set; }
    }
}