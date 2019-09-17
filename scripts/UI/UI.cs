using System;
using System.Collections.Generic;
using Godot;

namespace GameFeel.UI
{
    public class UI : CanvasLayer
    {
        [Signal]
        public delegate void CloseRequested();
        [Signal]
        public delegate void DeselectRequested();

        private HashSet<Node> _openedNodes = new HashSet<Node>();
        private HashSet<Type> _openedTypes = new HashSet<Type>();

        private Control _rootControl;

        public override void _Ready()
        {
            _rootControl = GetNode<Control>("Control");
            _rootControl.Connect("gui_input", this, nameof(OnGuiInput));

            foreach (var child in _rootControl.GetChildren())
            {
                if (child is ToggleUI tu)
                {
                    tu.Connect(nameof(ToggleUI.Closed), this, nameof(OnUIClosed));
                    tu.Connect(nameof(ToggleUI.Opened), this, nameof(OnUIOpened));
                }
            }
        }

        private void OnGuiInput(InputEvent evt)
        {
            if (evt.IsActionPressed("select") && _openedNodes.Count > 0)
            {
                EmitSignal(nameof(CloseRequested));
                _rootControl.AcceptEvent();
            }
            else if (evt.IsActionPressed("deselect") && (_openedTypes.Contains(typeof(PlayerInventoryUI)) || _openedTypes.Contains(typeof(EquipmentUI))))
            {
                EmitSignal(nameof(DeselectRequested));
                _rootControl.AcceptEvent();
            }
            else
            {
                Main.SendInput(evt);
            }
        }

        private void OnUIClosed(ToggleUI tu)
        {
            if (_openedNodes.Contains(tu))
            {
                _openedNodes.Remove(tu);
            }
            _openedTypes.Clear();
            foreach (var node in _openedNodes)
            {
                _openedTypes.Add(node.GetType());
            }
        }

        private void OnUIOpened(ToggleUI tu)
        {
            _openedNodes.Add(tu);
            _openedTypes.Add(tu.GetType());
        }
    }
}