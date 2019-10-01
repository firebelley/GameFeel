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
        public delegate void AttackStart(Player p);
        [Signal]
        public delegate void AttackEnd(Player p);
        [Signal]
        public delegate void Attacking(Player p);

        public const string GROUP = "player";

        private const string INPUT_MOVE_DOWN = "move_down";
        private const string INPUT_MOVE_LEFT = "move_left";
        private const string INPUT_MOVE_RIGHT = "move_right";
        private const string INPUT_MOVE_UP = "move_up";
        private const string INPUT_ATTACK = "attack";
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
        private Position2D _cameraTargetPosition2d;

        private HealthComponent _healthComponent;

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
            _cameraTargetPosition2d = GetNode<Position2D>("CameraTargetPosition2D");
            _healthComponent = GetNode<HealthComponent>("HealthComponent");

            _weaponRadius = _weaponPosition2d.Position.x;
            _weaponHeight = _weaponPosition2d.Position.y;

            AddToGroup(GROUP);

            PlayerInventory.Instance.Connect(nameof(PlayerInventory.ItemEquipped), this, nameof(OnItemEquipped));
            PlayerInventory.Instance.Connect(nameof(PlayerInventory.EquipmentCleared), this, nameof(OnEquipmentCleared));
            _healthComponent.Connect(nameof(HealthComponent.HealthDepleted), this, nameof(OnHealthDepleted));
            _healthComponent.Connect(nameof(HealthComponent.HealthDecremented), this, nameof(OnHealthDecremented));

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
            if (evt.IsActionPressed(INPUT_ATTACK))
            {
                GetTree().SetInputAsHandled();
                _attacking = true;
                EmitSignal(nameof(AttackStart), this);
            }
            else if (evt.IsActionReleased(INPUT_ATTACK))
            {
                GetTree().SetInputAsHandled();
                _attacking = false;
                EmitSignal(nameof(AttackEnd), this);
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
        }

        private Vector2 GetMovementVector()
        {
            var moveVec = Vector2.Zero;
            moveVec.x = Input.GetActionStrength(INPUT_MOVE_RIGHT) - Input.GetActionStrength(INPUT_MOVE_LEFT);
            moveVec.y = Input.GetActionStrength(INPUT_MOVE_DOWN) - Input.GetActionStrength(INPUT_MOVE_UP);
            return moveVec.Normalized();
        }

        private void OnItemEquipped(Equipment equipment)
        {
            // TODO: account for equipment slots here
            foreach (var child in _weaponPosition2d.GetChildren<Node>())
            {
                child.GetParent().RemoveChild(child);
                child.QueueFree();
            }

            _weaponPosition2d.AddChild(equipment);
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