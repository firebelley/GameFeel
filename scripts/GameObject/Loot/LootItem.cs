using Godot;

namespace GameFeel.GameObject.Loot
{
    public class LootItem : KinematicBody2D
    {
        private const string GROUP = "loot_item";
        private const string ANIM_IDLE = "idle";
        private const string ANIM_DEFAULT = "default";

        private const float SEPARATION_DISTANCE = 15f;
        private const float SEPARATION_SPEED = 15f;

        private const float MIN_ANGLE = 90f - 15f;
        private const float MAX_ANGLE = 90f + 15f;
        private const float MIN_SPEED = 200f;
        private const float MAX_SPEED = 250f;
        private const float DAMPING = .6f;
        private const int MAX_BOUNCES = 3;

        private const float GRAVITY = 300f;

        private AnimationPlayer _animationPlayer;
        private CollisionShape2D _collisionShape2d;
        private AnimationPlayer _blinkAnimationPlayer;
        private Timer _deathTimer;

        private float _floorY;
        private Vector2 _velocity;
        private int _bounces;

        public override void _Ready()
        {
            _collisionShape2d = GetNode<CollisionShape2D>("CollisionShape2D");
            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _blinkAnimationPlayer = GetNode<AnimationPlayer>("BlinkAnimationPlayer");
            _deathTimer = GetNode<Timer>("DeathTimer");
            _velocity = Vector2.Right.Rotated(Mathf.Deg2Rad(Main.RNG.RandfRange(MIN_ANGLE, MAX_ANGLE)));
            _velocity *= Main.RNG.RandfRange(MIN_SPEED, MAX_SPEED);
            AddToGroup(GROUP);

            _deathTimer.Connect("timeout", this, nameof(OnDeathTimerTimeout));
        }

        public override void _Process(float delta)
        {
            if (_bounces <= MAX_BOUNCES)
            {
                Bounce();
            }
            else
            {
                Separate();
            }
        }

        private void Bounce()
        {
            if (_floorY == 0f)
            {
                _floorY = GlobalPosition.y;
            }

            _velocity += Vector2.Down * GRAVITY * GetProcessDeltaTime();
            if (_velocity.y > 0f && GlobalPosition.y >= _floorY)
            {
                _bounces++;
                _velocity = new Vector2(1f, -1f) * _velocity * DAMPING;
            }

            _velocity = MoveAndSlide(_velocity);
        }

        private void Separate()
        {
            var pushVec = Vector2.Zero;
            foreach (var node in GetTree().GetNodesInGroup(GROUP))
            {
                if (node is Node2D n)
                {
                    if (GlobalPosition.DistanceSquaredTo(n.GlobalPosition) < SEPARATION_DISTANCE * SEPARATION_DISTANCE)
                    {
                        pushVec = (GlobalPosition - n.GlobalPosition);
                        break;
                    }
                }
            }

            if (pushVec == Vector2.Zero)
            {
                Disable();
            }
            else
            {
                pushVec = pushVec.Normalized() * SEPARATION_SPEED;
                _velocity = MoveAndSlide(pushVec);
            }
        }

        private void Disable()
        {
            CollisionLayer = 0;
            CollisionMask = 0;
            _velocity = Vector2.Zero;
            GlobalPosition = GlobalPosition.Round();
            SetProcess(false);
            _animationPlayer.Play(ANIM_IDLE);
            _collisionShape2d.Disabled = true;
            _deathTimer.Start();
        }

        private void OnDeathTimerTimeout()
        {
            _blinkAnimationPlayer.Play(ANIM_DEFAULT);
        }
    }
}