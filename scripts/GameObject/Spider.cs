using GameFeel.Component;
using Godot;
using GodotTools.Extension;
using GodotTools.Logic;

namespace GameFeel.GameObject
{
    public class Spider : KinematicBody2D
    {
        private const string ANIM_RUN = "run";
        private const string ANIM_ATTACK = "attack";
        private const string ANIM_IDLE = "idle";
        private const float ATTACK_RANGE = 40f;

        private AnimatedSprite _animatedSprite;
        private AnimationPlayer _animationPlayer;
        private ResourcePreloader _resourcePreloader;
        private Timer _attackDelayTimer;

        private HealthComponent _healthComponent;
        private PathfindComponent _pathfindComponent;
        private ProjectileSpawnComponent _projectileSpawnComponent;
        private AttackIntentComponent _attackIntentComponent;

        private StateMachine<State> _stateMachine = new StateMachine<State>();

        private enum State
        {
            PURSUE,
            SPAWN,
            ATTACK,
            ATTACK_PREPARATION
        }

        public override void _Ready()
        {
            _stateMachine.AddState(State.PURSUE, StatePursue);
            _stateMachine.AddState(State.SPAWN, StateSpawning);
            _stateMachine.AddState(State.ATTACK, StateAttack);
            _stateMachine.AddState(State.ATTACK_PREPARATION, StateAttackPreparation);
            _stateMachine.SetInitialState(State.SPAWN);

            _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _attackDelayTimer = GetNode<Timer>("AttackDelayTimer");

            _healthComponent = this.GetFirstNodeOfType<HealthComponent>();
            _pathfindComponent = this.GetFirstNodeOfType<PathfindComponent>();
            _projectileSpawnComponent = this.GetFirstNodeOfType<ProjectileSpawnComponent>();
            _attackIntentComponent = this.GetFirstNodeOfType<AttackIntentComponent>();

            _healthComponent.Connect(nameof(HealthComponent.HealthDepleted), this, nameof(OnHealthDepleted));
            _animatedSprite.Connect("animation_finished", this, nameof(OnAnimationFinished));
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
                _animatedSprite.Play(ANIM_RUN);
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

            var player = GetTree().GetFirstNodeInGroup<Player>(Player.GROUP);
            if (player != null)
            {
                if (player.GlobalPosition.DistanceSquaredTo(GlobalPosition) < ATTACK_RANGE * ATTACK_RANGE)
                {
                    _stateMachine.ChangeState(State.ATTACK_PREPARATION);
                }
            }
        }

        private void StateAttackPreparation(bool isStateNew)
        {
            if (isStateNew)
            {
                _attackDelayTimer.Start();
                _attackIntentComponent.Play();
                _animatedSprite.Play(ANIM_IDLE);
            }

            if (_attackDelayTimer.IsStopped())
            {
                _attackIntentComponent.Stop();
                _stateMachine.ChangeState(State.ATTACK);
            }
        }

        private void StateAttack(bool isStateNew)
        {
            if (isStateNew)
            {
                _animatedSprite.Play(ANIM_ATTACK);
            }
        }

        private void Kill()
        {
            var death = _resourcePreloader.InstanceScene<Node2D>("EntityDeath");
            GameZone.EffectsLayer.AddChild(death);
            death.GlobalPosition = GlobalPosition;
            QueueFree();
        }

        private void OnHealthDepleted()
        {
            Kill();
        }

        private void OnAnimationFinished()
        {
            if (_animatedSprite.Animation == ANIM_ATTACK)
            {
                _stateMachine.ChangeState(State.PURSUE);

                var player = GetTree().GetFirstNodeInGroup<Player>(Player.GROUP);
                var toPos = Vector2.Zero;
                if (player != null)
                {
                    toPos = player.GetFirstNodeOfType<DamageReceiverComponent>()?.GlobalPosition ?? player.GlobalPosition;
                }

                _projectileSpawnComponent.SpawnToPosition(toPos);
            }
        }
    }
}