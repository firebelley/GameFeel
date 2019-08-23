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
        private NodePath _shadedNodePath;

        [Export]
        private bool _requirePlayerNear;
        [Export]
        private int _playerNearDistance = 75;

        private static SelectableComponent _selected;

        private Node2D _shadedNode;
        private bool _valid;

        public override void _Ready()
        {
            if (!Engine.IsEditorHint() && _shadedNodePath != null)
            {
                _shadedNode = GetNode<Node2D>(_shadedNodePath);
                _shadedNode.Material = Material.Duplicate() as Material;
            }

            GetTree().GetFirstNodeInGroup<Player>(Player.GROUP)?.Connect(nameof(Player.Interact), this, nameof(OnPlayerInteract));

            Connect("mouse_entered", this, nameof(OnMouseEntered));
            Connect("mouse_exited", this, nameof(OnMouseExited));
            Connect("input_event", this, nameof(OnInputEvent));

            SetProcess(false);
        }

        public override void _Process(float delta)
        {
            var playerPos = GetTree().GetFirstNodeInGroup<Player>(Player.GROUP)?.GlobalPosition ?? Vector2.Zero;
            var prevValid = _valid;
            _valid = !_requirePlayerNear || GlobalPosition.DistanceSquaredTo(playerPos) <= _playerNearDistance * _playerNearDistance;

            if (!prevValid == _valid)
            {
                ToggleHighlight(true);
            }
        }

        public void ToggleHighlight(bool hovered)
        {
            if (IsInstanceValid(_shadedNode))
            {
                var material = _shadedNode.Material as ShaderMaterial;
                material.SetShaderParam("_enabled", hovered);
                material.SetShaderParam("_valid", _valid);
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

        public void Deselect()
        {
            if (_selected == this)
            {
                _selected = null;
                EmitSignal(nameof(SelectLeave));
            }
            ToggleHighlight(false);
            SetProcess(false);
            _valid = false;
        }

        public void Select()
        {
            var wasSelected = _selected;
            if (IsInstanceValid(_selected) && _selected != this)
            {
                _selected.Deselect();
            }
            _selected = this;
            ToggleHighlight(true);
            SetProcess(true);

            if (wasSelected != this)
            {
                EmitSignal(nameof(SelectEnter));
            }
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
            if (_selected == this && _valid)
            {
                EmitSignal(nameof(Selected));
            }
        }
    }
}