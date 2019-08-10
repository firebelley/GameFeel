using GameFeel.GameObject;
using Godot;
using GodotTools.Extension;

namespace GameFeel.Component
{
    [Tool]
    public class SelectableComponent : Area2D
    {
        [Signal]
        public delegate void Selected();
        [Signal]
        public delegate void SelectEnter();
        [Signal]
        public delegate void SelectLeave();

        [Export]
        private ShaderMaterial _shaderMaterial;
        [Export]
        private NodePath _shadedNodePath;

        private static SelectableComponent _selected;

        private Node2D _shadedNode;
        private bool _hovered;

        public override void _Ready()
        {
            if (_shadedNodePath != null)
            {
                _shadedNode = GetNode<Node2D>(_shadedNodePath);
                _shadedNode.Material = _shaderMaterial;
            }

            GetTree().GetFirstNodeInGroup<Player>(Player.GROUP)?.Connect(nameof(Player.Interact), this, nameof(OnPlayerInteract));

            Connect("mouse_entered", this, nameof(OnMouseEntered));
            Connect("mouse_exited", this, nameof(OnMouseExited));
            Connect("input_event", this, nameof(OnInputEvent));
        }

        public void ToggleHighlight(bool hovered)
        {
            if (IsInstanceValid(_shadedNode))
            {
                var material = _shadedNode.Material as ShaderMaterial;
                material.SetShaderParam("_enabled", hovered);
            }
        }

        public void Disable()
        {
            var collisionShape = this.GetFirstNodeOfType<CollisionShape2D>();
            if (collisionShape != null)
            {
                collisionShape.Disabled = true;
            }
            Deselect();
        }

        private void Select()
        {
            if (IsInstanceValid(_selected))
            {
                _selected.ToggleHighlight(false);
            }
            var wasSelected = _selected;
            _selected = this;
            ToggleHighlight(true);

            if (wasSelected != _selected)
            {
                EmitSignal(nameof(SelectEnter));
            }
        }

        private void Deselect()
        {
            if (_selected == this)
            {
                _selected = null;
            }
            ToggleHighlight(false);
            EmitSignal(nameof(SelectLeave));
        }

        private void OnMouseEntered()
        {
            Select();
        }

        private void OnMouseExited()
        {
            Deselect();
        }

        private void OnInputEvent(Node viewport, InputEvent evt, int shapeIdx)
        {
            if (evt is InputEventMouse)
            {
                Select();
            }
        }

        private void OnPlayerInteract()
        {
            if (_selected == this)
            {
                EmitSignal(nameof(Selected));
            }
        }
    }
}