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
        private Timer _attackDelayTimer;

        private PathfindComponent _pathfindComponent;
        private ProjectileSpawnComponent _projectileSpawnComponent;
        private AttackIntentComponent _attackIntentComponent;

        private StateMachine<State> _stateMachine = new StateMachine<State>();

        private Vector2 _spawnPosition;

        private enum State
        {
            PURSUE,
            SPAWN,
            ATTACK,
            ATTACK_PREPARATION,
            WANDER
        }

        public override void _Ready()
        {
            _stateMachine.AddState(State.PURSUE, StatePursue);
            _stateMachine.AddState(State.SPAWN, StateSpawning);
            _stateMachine.AddState(State.ATTACK, StateAttack);
            _stateMachine.AddState(State.ATTACK_PREPARATION, StateAttackPreparation);
            _stateMachine.AddState(State.WANDER, StateWander);
            _stateMachine.AddLeaveState(State.ATTACK, LeaveStateAttack);
            _stateMachine.AddLeaveState(State.PURSUE, LeaveStatePursue);
            _stateMachine.SetInitialState(State.SPAWN);

            _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _attackDelayTimer = GetNode<Timer>("AttackDelayTimer");

            _pathfindComponent = this.GetFirstNodeOfType<PathfindComponent>();
            _projectileSpawnComponent = this.GetFirstNodeOfType<ProjectileSpawnComponent>();
            _attackIntentComponent = this.GetFirstNodeOfType<AttackIntentComponent>();

            _animatedSprite.Connect("animation_finished", this, nameof(OnAnimationFinished));
        }

        public override void _Process(float delta)
        {
            _stateMachine.Update();
        }

        private void StateSpawning()
        {
            if (_stateMachine.IsStateNew())
            {
                _animatedSprite.FlipH = Main.RNG.RandiRange(0, 1) == 1;
                _spawnPosition = GlobalPosition;
            }

            if (!_animationPlayer.IsPlaying())
            {
                _stateMachine.ChangeState(StateWander);
            }
        }

        private void StatePursue()
        {
            if (_stateMachine.IsStateNew())
            {
                _pathfindComponent.EnablePathTimer();
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
                    _stateMachine.ChangeState(StateAttackPreparation);
                }
            }
        }

        private void LeaveStatePursue()
        {
            _pathfindComponent.DisablePathTimer();
        }

        private void StateAttackPreparation()
        {
            if (_stateMachine.IsStateNew())
            {
                _attackDelayTimer.WaitTime = .25f;
                _attackDelayTimer.Start();
                _attackIntentComponent.Play();
                _animatedSprite.Play(ANIM_IDLE);
            }

            if (_attackDelayTimer.IsStopped())
            {
                _stateMachine.ChangeState(StateAttack);
            }
        }

        private void StateAttack()
        {
            if (_stateMachine.IsStateNew())
            {
                _animatedSprite.Play(ANIM_ATTACK);
            }
        }

        private void LeaveStateAttack()
        {
            _attackIntentComponent.Stop();
        }

        private void StateWander()
        {
            if (_stateMachine.IsStateNew())
            {
                _pathfindComponent.DisablePathTimer();
            }

            if (_attackDelayTimer.IsStopped())
            {
                var toPos = Vector2.Right.Rotated(Main.RNG.RandfRange(0f, Mathf.Pi * 2f));
                toPos *= 100f;
                toPos += _spawnPosition;
                _pathfindComponent.UpdatePath(GlobalPosition, toPos);
                _attackDelayTimer.WaitTime = Main.RNG.RandfRange(1.5f, 2.5f);
                _attackDelayTimer.Start();
            }

            if (GlobalPosition.DistanceSquaredTo(GetTree().GetFirstNodeInGroup<Player>(Player.GROUP)?.GlobalPosition ?? Vector2.Zero) < 5000f)
            {
                _stateMachine.ChangeState(StatePursue);
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

            if (_pathfindComponent.Velocity.LengthSquared() > 4f)
            {
                _animatedSprite.Play(ANIM_RUN);
            }
            else
            {
                _animatedSprite.Play(ANIM_IDLE);
            }
        }

        private void OnAnimationFinished()
        {
            if (_animatedSprite.Animation == ANIM_ATTACK)
            {
                _stateMachine.ChangeState(StatePursue);

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