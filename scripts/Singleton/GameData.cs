using System.Collections.Generic;
using Godot;

namespace GameFeel.Singleton
{
    public class GameData : Node
    {
        public static HashSet<string> DoNotSpawnEntityIds = new HashSet<string>();
    }
}