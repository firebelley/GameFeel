using System;
using GameFeel.Singleton;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestEventEntityKilled : QuestEventNode
    {
        public override void _Ready()
        {
            base._Ready();
            ((QuestEventModel) Model).EventId = GameEventDispatcher.ENTITY_KILLED;
        }
    }
}