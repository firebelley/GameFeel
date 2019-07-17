using GameFeel.Component;
using GameFeel.Interface;
using Godot;
using GodotTools.Extension;
using GodotTools.Logic;

namespace GameFeel.GameObject
{
    public class Spider : KinematicBody2D, IDamageReceiver
    {
        private const float MAX_AHEAD = 20f;
        private const float ACCELERATION = 400f;

        [Signal]
        public delegate void DamageReceived(float damage);

        private Area2D _hitboxArea;
        private Tween _shaderTween;
        private AnimatedSprite _animatedSprite;
        private ShaderMaterial _shaderMaterial;
        private AnimationPlayer _animationPlayer;
        private Timer _pathfindTimer;
        private HealthComponent _healthComponent;

        private float _currentT;
        private float _speed = 75f;
        private Curve2D _curve = new Curve2D();
        private Vector2 _velocity;

        private ResourcePreloader _resourcePreloader;

        private StateMachine<State> _stateMachine = new StateMachine<State>();

        private enum State
        {
            PURSUE,
            SPAWNING
        }

        public override void _Ready()
        {
            _stateMachine.AddState(State.PURSUE, StatePursue);
            _stateMachine.AddState(State.SPAWNING, StateSpawning);
            _stateMachine.SetInitialState(State.SPAWNING);

            _hitboxArea = GetNode<Area2D>("HitboxArea2D");
            _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            _shaderTween = GetNode<Tween>("ShaderTween");
            _pathfindTimer = GetNode<Timer>("PathfindTimer");
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
            _healthComponent = GetNode<HealthComponent>("HealthComponent");
            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

            _shaderMaterial = _animatedSprite.Material as ShaderMaterial;
            _hitboxArea.Connect("body_entered", this, nameof(OnBodyEntered));
            _pathfindTimer.Connect("timeout", this, nameof(OnPathfindTimerTimeout));
            _healthComponent.Connect(nameof(HealthComponent.HealthDepleted), this, nameof(OnHealthDepleted));
        }

        public override void _Process(float delta)
        {
            _stateMachine.Update();
        }

        public void DealDamage(float damage)
        {
            PlayHitShadeTween();
            Camera.Shake();
            GameWorld.CreateDamageNumber(this, damage);
            _healthComponent.Decrement(damage);
            EmitSignal(nameof(DamageReceived), damage);
        }

        private void StateSpawning(bool isStateNew)
        {
            if (isStateNew)
            {
                _animatedSprite.FlipH = Main.RNG.RandiRange(0, 1) == 1;
            }

            if (!_animationPlayer.IsPlaying())
            {
                _stateMachine.ChangeState(State.PURSUE);
            }
        }

        private void StatePursue(bool isStateNew)
        {
            if (isStateNew)
            {
                UpdatePath();
            }
            var destinationPoint = _curve.InterpolateBaked(_currentT);
            var acceleration = Vector2.Zero;

            if (GlobalPosition.DistanceSquaredTo(destinationPoint) < MAX_AHEAD * MAX_AHEAD)
            {
                _currentT += _speed * GetProcessDeltaTime();
            }

            if (_currentT < (_curve.GetBakedLength()))
            {
                acceleration = (destinationPoint - GlobalPosition).Normalized() * ACCELERATION;
            }
            else
            {
                _velocity = Vector2.Zero;
            }

            _velocity += acceleration * GetProcessDeltaTime();
            _velocity = _velocity.Clamped(_speed);

            _velocity = MoveAndSlide(_velocity);
            if (_velocity.x < -5f)
            {
                _animatedSprite.FlipH = true;
            }
            else if (_velocity.x > 5f)
            {
                _animatedSprite.FlipH = false;
            }
        }

        private void Kill()
        {
            var death = _resourcePreloader.InstanceScene<Node2D>("EntityDeath");
            GameWorld.EffectsLayer.AddChild(death);
            death.GlobalPosition = GlobalPosition;
            QueueFree();
        }

        private void PlayHitShadeTween()
        {
            _shaderTween.ResetAll();
            _shaderTween.InterpolateProperty(
                _shaderMaterial,
                "shader_param/_hitShadePercent",
                1.0f,
                0f,
                .3f,
                Tween.TransitionType.Quad,
                Tween.EaseType.In
            );
            _shaderTween.Start();
        }

        private void UpdatePath()
        {
            var player = GetTree().GetFirstNodeInGroup<Player>(Player.GROUP);
            if (player != null)
            {
                _curve = GameWorld.GetPathCurve(GlobalPosition, player.GlobalPosition);
                _currentT = 0f;
            }
        }

        private void OnBodyEntered(PhysicsBody2D body)
        {
            if (body is IDamageDealer dd)
            {
                dd.RegisterHit(this);
            }
        }

        private void OnPathfindTimerTimeout()
        {
            if (_stateMachine.GetCurrentState() == State.PURSUE)
            {
                UpdatePath();
            }
        }

        private void OnHealthDepleted()
        {
            Kill();
        }
    }
}