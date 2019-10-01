using System.Collections.Generic;
using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;

namespace GameFeel.UI
{
    public class TutorialUI : Control
    {
        private Stack<string> _instructions = new Stack<string>();

        [Export]
        private NodePath _labelPath;

        private Label _label;

        public override void _Ready()
        {
            this.SetNodesByDeclaredNodePaths();

            _instructions.Push("Destroy the barrier");
            _instructions.Push("Open your inventory with 'I'\nOpen your equipment with 'C'\nPlace the tome in the equipment slot");
            _instructions.Push("Pick up the Fire Tome");
            NextInstruction();

            GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventPlayerInventoryItemUpdated), this, nameof(OnPlayerInventoryItemUpdated));
            GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventEntityKilled), this, nameof(OnEntityKilled));
            PlayerInventory.Instance.Connect(nameof(PlayerInventory.EquipmentUpdated), this, nameof(OnEquipmentUpdated));
        }

        private void NextInstruction()
        {
            if (_instructions.Count == 0)
            {
                QueueFree();
            }
            else
            {
                _label.Text = _instructions.Pop();
                _label.RectPivotOffset = _label.RectSize / 2f;
            }
        }

        private void OnPlayerInventoryItemUpdated(string eventGuid, string itemId)
        {
            NextInstruction();
            GameEventDispatcher.Instance.Disconnect(nameof(GameEventDispatcher.EventPlayerInventoryItemUpdated), this, nameof(OnPlayerInventoryItemUpdated));
        }

        private void OnEntityKilled(string eventGuid, string entityGuid)
        {
            NextInstruction();
            GameEventDispatcher.Instance.Disconnect(nameof(GameEventDispatcher.EventEntityKilled), this, nameof(OnEntityKilled));
        }

        private void OnEquipmentUpdated(int slotIdx)
        {
            NextInstruction();
            PlayerInventory.Instance.Disconnect(nameof(PlayerInventory.EquipmentUpdated), this, nameof(OnEquipmentUpdated));
        }
    }
}