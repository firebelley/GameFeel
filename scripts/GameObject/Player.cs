using GameFeel.Component;
using GameFeel.GameObject.Loot;
using GameFeel.Resource;
using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;

namespace GameFeel.GameObject
{
    public class Player : KinematicBody2D
    {
        [Signal]
        public delegate void Attacking(Player p);

        public const string GROUP = "player";

        private const string INPUT_MOVE_DOWN = "move_down";
        private const string INPUT_MOVE_LEFT = "move_left";
        private const string INPUT_MOVE_RIGHT = "move_right";
        private const string INPUT_MOVE_UP = "move_up";
        private const string INPUT_MAIN_ATTACK = "main_attack";
        private const string INPUT_SECONDARY_ATTACK = "secondary_attack";
        private const string INPUT_INTERACT = "interact";

        private const string ANIM_IDLE = "idle";
        private const string ANIM_RUN = "run";

        private const float ACCELERATION = 3000f;
        private const float DECELERATION = 17f;
        private const float MAX_SPEED = 125f;
        private const float MANA_REGEN_RATE = 5f;

        private Vector2 _velocity;
        private AnimatedSprite _animatedSprite;
        private Position2D _weaponPosition2d;
        private Position2D _secondaryWeaponPosition2d;
        private Node2D _secondaryWeapon;
        private Position2D _cameraTargetPosition2d;

        private HealthComponent _healthComponent;

        private Equipment[] _equippedItems = new Equipment[2];
        private bool[] _attackMouseButtons = new bool[2];

        private float _weaponRadius;
        private float _weaponHeight;
        private bool _attacking;

        public float Mana { get; private set; } = 15f;
        public float MaxMana { get; private set; } = 15f;
        public float Health
        {
            get
            {
                return _healthComponent?.CurrentHp ?? -1f;
            }
        }
        public float MaxHealth
        {
            get
            {
                return _healthComponent?.MaxHp ?? -1f;
            }
        }

        public override void _Ready()
        {
            _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            _weaponPosition2d = GetNode<Position2D>("WeaponPosition2D");
            _secondaryWeapon = GetNode<Node2D>("SecondaryWeapon");
            _secondaryWeaponPosition2d = GetNode<Position2D>("SecondaryWeapon/SecondaryWeaponPosition2D");
            _cameraTargetPosition2d = GetNode<Position2D>("CameraTargetPosition2D");
            _healthComponent = GetNode<HealthComponent>("HealthComponent");

            _weaponRadius = _weaponPosition2d.Position.x;
            _weaponHeight = _weaponPosition2d.Position.y;

            AddToGroup(GROUP);

            PlayerInventory.Instance.Connect(nameof(PlayerInventory.ItemEquipped), this, nameof(OnItemEquipped));
            PlayerInventory.Instance.Connect(nameof(PlayerInventory.EquipmentCleared), this, nameof(OnEquipmentCleared));
            _healthComponent.Connect(nameof(HealthComponent.HealthDepleted), this, nameof(OnHealthDepleted));
            _healthComponent.Connect(nameof(HealthComponent.HealthDecremented), this, nameof(OnHealthDecremented));

            UpdateEquipment();
            GameEventDispatcher.DispatchPlayerCreatedEvent(this);
        }

        public override void _Process(float delta)
        {
            UpdateMovement();
            UpdateRegen();
            UpdateWeaponOrientation();

            if (Input.IsActionJustPressed(INPUT_INTERACT))
            {
                GameEventDispatcher.DispatchPlayerInteractEvent();
            }

            if (_attacking)
            {
                EmitSignal(nameof(Attacking), this);
            }
        }

        public override void _UnhandledInput(InputEvent evt)
        {
            if (evt.IsActionPressed(INPUT_MAIN_ATTACK) || evt.IsActionPressed(INPUT_SECONDARY_ATTACK))
            {
                var buttonIndex = evt.IsActionPressed(INPUT_MAIN_ATTACK) ? 0 : 1;
                _attackMouseButtons[buttonIndex] = true;
                SwapToWeapon(buttonIndex);
                GetTree().SetInputAsHandled();
                _attacking = true;
            }
            else if (evt.IsActionReleased(INPUT_MAIN_ATTACK) || evt.IsActionReleased(INPUT_SECONDARY_ATTACK))
            {
                var buttonIndex = evt.IsActionReleased(INPUT_MAIN_ATTACK) ? 0 : 1;
                _attackMouseButtons[buttonIndex] = false;
                GetTree().SetInputAsHandled();

                for (int i = 0; i < _attackMouseButtons.Length; i++)
                {
                    if (_attackMouseButtons[i])
                    {
                        SwapToWeapon(i);
                        return;
                    }
                }
                _attacking = false;
            }
        }

        public Position2D GetWeaponPosition()
        {
            return _weaponPosition2d;
        }

        public void RemoveMana(float amount)
        {
            AddMana(-amount);
        }

        public Vector2 GetCameraTargetPosition()
        {
            return _cameraTargetPosition2d.GlobalPosition;
        }

        private void UpdateMovement()
        {
            var delta = GetProcessDeltaTime();
            var moveVec = GetMovementVector();
            if (moveVec.LengthSquared() != 0f)
            {
                _velocity += moveVec * ACCELERATION * delta;
                _animatedSprite.Play(ANIM_RUN);
            }
            else
            {
                _velocity = _velocity.LinearInterpolate(Vector2.Zero, DECELERATION * delta);
                _animatedSprite.Play(ANIM_IDLE);
            }

            _velocity = _velocity.Clamped(MAX_SPEED);
            _velocity = MoveAndSlide(_velocity);

            var spriteScale = _animatedSprite.Scale;
            spriteScale.x = Cursor.GetAdjustedGlobalMousePosition(this).x < GlobalPosition.x ? -1f : 1f;
            _animatedSprite.Scale = spriteScale;
        }

        private void AddMana(float amount)
        {
            var prevMana = Mana;
            Mana = Mathf.Clamp(Mana + amount, 0f, MaxMana);
            if (!Mathf.IsEqualApprox(prevMana, Mana))
            {
                GameEventDispatcher.DispatchPlayerManaChangedEvent(this);
            }
        }

        private void UpdateRegen()
        {
            var delta = GetProcessDeltaTime();
            AddMana(MANA_REGEN_RATE * delta);
        }

        private void UpdateWeaponOrientation()
        {
            var centerPosition = new Vector2(0, _weaponHeight);
            var globalCenterPosition = GlobalPosition + centerPosition;
            var direction = (Cursor.GetAdjustedGlobalMousePosition(this) - globalCenterPosition).Normalized();
            _weaponPosition2d.GlobalPosition = globalCenterPosition + _weaponRadius * direction;
            _weaponPosition2d.Rotation = direction.Angle();

            var weaponScale = _weaponPosition2d.Scale;
            weaponScale.y = Mathf.Sign(_animatedSprite.Scale.x);
            _weaponPosition2d.Scale = weaponScale;

            _secondaryWeapon.Scale = _animatedSprite.Scale;
            _secondaryWeaponPosition2d.Scale = new Vector2(-1, 1);
        }

        private Vector2 GetMovementVector()
        {
            var moveVec = Vector2.Zero;
            moveVec.x = Input.GetActionStrength(INPUT_MOVE_RIGHT) - Input.GetActionStrength(INPUT_MOVE_LEFT);
            moveVec.y = Input.GetActionStrength(INPUT_MOVE_DOWN) - Input.GetActionStrength(INPUT_MOVE_UP);
            return moveVec.Normalized();
        }

        private void UpdateEquipment()
        {
            for (int i = 0; i < PlayerInventory.EquipmentSlots.Length; i++)
            {
                var equipment = PlayerInventory.EquipmentSlots[i];
                if (equipment == null) continue;

                var scene = PlayerInventory.CreateEquipmentScene(equipment);
                if (IsInstanceValid(scene))
                {
                    EquipItem(scene, i);
                }
            }
        }

        private void EquipItem(Equipment equipment, int slot)
        {
            if (IsInstanceValid(_equippedItems[slot]))
            {
                _equippedItems[slot].GetParent().RemoveChild(_equippedItems[slot]);
                _equippedItems[slot].QueueFree();
                _equippedItems[slot] = null;
            }
            _equippedItems[slot] = equipment;
            PrepareWeapon(0);
        }

        private void SwapToWeapon(int index)
        {
            if (_weaponPosition2d.GetChildren().Count > 0 && (_weaponPosition2d.GetChild(0) == _equippedItems[index] || !IsInstanceValid(_equippedItems[index])))
            {
                return;
            }
            PrepareWeapon(index);
        }

        private void PrepareWeapon(int index)
        {
            foreach (var item in _equippedItems)
            {
                if (IsInstanceValid(item) && item.IsInsideTree())
                {
                    item.DisconnectAllSignals(this);
                    item.GetParent().RemoveChild(item);
                }
            }

            var desiredWeapon = _equippedItems[index];
            var secondWeapon = index + 1 > 1 ? _equippedItems[0] : _equippedItems[1];
            if (IsInstanceValid(desiredWeapon))
            {
                _weaponPosition2d.AddChild(desiredWeapon);
                desiredWeapon.ConnectSignals(this);
                if (IsInstanceValid(secondWeapon))
                {
                    _secondaryWeaponPosition2d.AddChild(secondWeapon);
                }
            }
            else if (IsInstanceValid(secondWeapon))
            {
                _weaponPosition2d.AddChild(secondWeapon);
            }
        }

        private void OnItemEquipped(Equipment equipment, int slot)
        {
            EquipItem(equipment, slot);
        }

        private void OnEquipmentCleared(int slotIdx)
        {
            foreach (var child in _weaponPosition2d.GetChildren<Node>())
            {
                child.GetParent().RemoveChild(child);
                child.QueueFree();
            }
        }

        private void OnHealthDepleted()
        {
            GameEventDispatcher.DispatchPlayerDiedEvent();
            QueueFree();
        }

        private void OnHealthDecremented()
        {
            GameEventDispatcher.DispatchPlayerHealthChangedEvent(this);
        }
    }
}