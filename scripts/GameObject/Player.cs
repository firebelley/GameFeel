using Godot;

namespace GameFeel.GameObject
{
    public class Player : KinematicBody2D
    {
        private const string INPUT_MOVE_DOWN = "move_down";
        private const string INPUT_MOVE_LEFT = "move_left";
        private const string INPUT_MOVE_RIGHT = "move_right";
        private const string INPUT_MOVE_UP = "move_up";
        private const string INPUT_ATTACK = "attack";
        private const string ANIM_IDLE = "idle";
        private const string ANIM_RUN = "run";

        private const float ACCELERATION = 3000f;
        private const float DECELERATION = 15f;
        private const float MAX_SPEED = 125f;

        private Vector2 _velocity;
        private AnimatedSprite _animatedSprite;
        private Position2D _position2d;

        // TODO: move to resource preloader
        private PackedScene _fireballScene;

        public override void _Ready()
        {
            _fireballScene = GD.Load("res://scenes/GameObject/Fireball.tscn") as PackedScene;
            _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            _position2d = GetNode<Position2D>("AnimatedSprite/Position2D");
        }

        public override void _Process(float delta)
        {
            UpdateMovement(delta);
            UpdateAttack(delta);
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
                var fireball = _fireballScene.Instance() as Fireball;
                Main.EffectsLayer.AddChild(fireball);
                var position = _position2d.GlobalPosition;
                fireball.SetDirection(GetGlobalMousePosition() - position);
                fireball.GlobalPosition = position;
            }
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