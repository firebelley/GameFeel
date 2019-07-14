using Godot;

namespace GameFeel.GameObject
{
    public class Player : KinematicBody2D
    {
        public const string GROUP = "player";

        private const string INPUT_MOVE_DOWN = "move_down";
        private const string INPUT_MOVE_LEFT = "move_left";
        private const string INPUT_MOVE_RIGHT = "move_right";
        private const string INPUT_MOVE_UP = "move_up";
        private const string INPUT_ATTACK = "attack";
        private const string ANIM_IDLE = "idle";
        private const string ANIM_RUN = "run";

        private const float ACCELERATION = 3000f;
        private const float DECELERATION = 17f;
        private const float MAX_SPEED = 125f;
        private const float MANA_REGEN_RATE = 2f;

        private Vector2 _velocity;
        private AnimatedSprite _animatedSprite;
        private Position2D _position2d;

        public float Mana { get; private set; } = 15f;
        public float MaxMana { get; private set; } = 15f;

        // TODO: move to resource preloader
        private PackedScene _fireballScene;

        public override void _Ready()
        {
            _fireballScene = GD.Load("res://scenes/GameObject/Fireball.tscn") as PackedScene;
            _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            _position2d = GetNode<Position2D>("AnimatedSprite/Position2D");

            AddToGroup(GROUP);
        }

        public override void _Process(float delta)
        {
            UpdateMovement(delta);
            UpdateAttack(delta);
            UpdateRegen(delta);
        }

        private void UpdateMovement(float delta)
        {
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
            spriteScale.x = GetGlobalMousePosition().x < GlobalPosition.x ? -1f : 1f;
            _animatedSprite.Scale = spriteScale;
        }

        private void UpdateAttack(float delta)
        {
            if (Input.IsActionJustPressed(INPUT_ATTACK))
            {
                if (Mana >= 1f)
                {
                    var fireball = _fireballScene.Instance() as Fireball;
                    Main.EffectsLayer.AddChild(fireball);
                    var position = _position2d.GlobalPosition;
                    fireball.SetDirection(GetGlobalMousePosition() - position);
                    fireball.GlobalPosition = position;
                    Mana -= 1f;
                }
            }
        }

        private void UpdateRegen(float delta)
        {
            Mana = Mathf.Clamp(Mana + MANA_REGEN_RATE * delta, 0f, MaxMana);
        }

        private Vector2 GetMovementVector()
        {
            var moveVec = Vector2.Zero;
            moveVec.x = Input.GetActionStrength(INPUT_MOVE_RIGHT) - Input.GetActionStrength(INPUT_MOVE_LEFT);
            moveVec.y = Input.GetActionStrength(INPUT_MOVE_DOWN) - Input.GetActionStrength(INPUT_MOVE_UP);
            return moveVec.Normalized();
        }
    }
}