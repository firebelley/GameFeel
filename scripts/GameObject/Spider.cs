using GameFeel.Component;
using Godot;
using GodotTools.Extension;
using GodotTools.Logic;

namespace GameFeel.GameObject
{
    public class Spider : KinematicBody2D
    {
        private AnimatedSprite _animatedSprite;
        private AnimationPlayer _animationPlayer;
        private ResourcePreloader _resourcePreloader;

        private HealthComponent _healthComponent;
        private PathfindComponent _pathfindComponent;

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

            _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

            _healthComponent = GetNode<HealthComponent>("HealthComponent");
            _pathfindComponent = GetNode<PathfindComponent>("PathfindComponent");

            _healthComponent.Connect(nameof(HealthComponent.HealthDepleted), this, nameof(OnHealthDepleted));
        }

        public override void _Process(float delta)
        {
            _stateMachine.Update();
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
                _pathfindComponent.UpdatePath();
            }

            _pathfindComponent.UpdateVelocity();

            if (_pathfindComponent.Velocity.x < -5f)
            {
                _animatedSprite.FlipH = true;
            }
            else if (_pathfindComponent.Velocity.x > 5f)
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

        private void OnHealthDepleted()
        {
            Kill();
        }
    }
}