using System;
using GameFeel.Singleton;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestEventPlayerInventoryItemAdded : QuestEventNode
    {
        public override void _Ready()
        {
            base._Ready();
            ((QuestEventModel) Model).EventId = GameEventDispatcher.PLAYER_INVENTORY_ITEM_ADDED;
        }
    }
}