using GameFeel.Component;
using GameFeel.Singleton;
using GameFeel.UI;
using Godot;
using GodotTools.Extension;
using GodotTools.Logic;

namespace GameFeel.GameObject.Loot
{
    public class LootItem : KinematicBody2D
    {
        private const string GROUP = "loot_item";
        private const string ANIM_IDLE = "idle";
        private const string ANIM_PICKUP = "pickup";
        private const string ANIM_DEFAULT = "default";
        private const float PLAYER_NEAR_DISTANCE = 50f;
        private const float PLAYER_HOVER_Y_OFFSET = -24f;

        private const float SEPARATION_DISTANCE = 30f;
        private const float SEPARATION_SPEED = 30f;

        private const float MIN_ANGLE = 90f - 15f;
        private const float MAX_ANGLE = 90f + 15f;
        private const float MIN_SPEED = 200f;
        private const float MAX_SPEED = 250f;
        private const float DAMPING = .6f;
        private const float GRAVITY = 300f;
        private const int MAX_BOUNCES = 3;

        [Export]
        public string Id { get; private set; }

        [Export]
        public Texture Icon
        {
            get
            {
                return _spriteTexture;
            }
            private set
            {
                _spriteTexture = value;
                if (IsInstanceValid(_sprite))
                {
                    _sprite.Texture = _spriteTexture;
                }
            }
        }

        [Export]
        public string DisplayName { get; private set; }

        [Export]
        public PackedScene EquipmentScene { get; private set; }

        private StateMachine<State> _stateMachine = new StateMachine<State>();

        private AnimationPlayer _animationPlayer;
        private CollisionShape2D _collisionShape2d;
        private AnimationPlayer _blinkAnimationPlayer;
        private Timer _deathTimer;
        private SelectableComponent _selectableComponent;
        private Sprite _sprite;
        private Texture _spriteTexture;

        private float _floorY;
        private Vector2 _velocity;
        private int _bounces;

        private enum State
        {
            BOUNCING,
            SEPARATE,
            SETTLED,
            PICKED_UP
        }

        public override void _Ready()
        {
            _stateMachine.AddState(State.BOUNCING, StateBouncing);
            _stateMachine.AddState(State.SEPARATE, StateSeparate);
            _stateMachine.AddState(State.SETTLED, StateSettled);
            _stateMachine.AddState(State.PICKED_UP, StatePickedUp);
            _stateMachine.SetInitialState(State.BOUNCING);

            _collisionShape2d = GetNode<CollisionShape2D>("CollisionShape2D");
            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _blinkAnimationPlayer = GetNode<AnimationPlayer>("BlinkAnimationPlayer");
            _deathTimer = GetNode<Timer>("DeathTimer");
            _selectableComponent = GetNode<SelectableComponent>("SelectableComponent");

            _sprite = GetNode<Sprite>("Sprite");
            _sprite.Texture = _spriteTexture;

            _velocity = Vector2.Right.Rotated(Mathf.Deg2Rad(Main.RNG.RandfRange(MIN_ANGLE, MAX_ANGLE)));
            _velocity *= Main.RNG.RandfRange(MIN_SPEED, MAX_SPEED);

            AddToGroup(GROUP);
            _deathTimer.Connect("timeout", this, nameof(OnDeathTimerTimeout));
            _selectableComponent.Connect(nameof(SelectableComponent.Selected), this, nameof(OnSelected));
            _selectableComponent.Connect(nameof(SelectableComponent.SelectEnter), this, nameof(OnSelectEnter));
            _selectableComponent.Connect(nameof(SelectableComponent.SelectLeave), this, nameof(OnSelectLeave));

            if (Engine.IsEditorHint())
            {
                SetProcess(false);
            }
        }

        public override void _Process(float delta)
        {
            _stateMachine.Update();
        }

        private void StateBouncing()
        {
            if (_bounces <= MAX_BOUNCES)
            {
                Bounce();
            }
            else
            {
                _stateMachine.ChangeState(StateSeparate);
            }
        }

        private void StateSeparate()
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
                _stateMachine.ChangeState(StateSettled);
            }
            else
            {
                pushVec = pushVec.Normalized() * SEPARATION_SPEED;
                _velocity = MoveAndSlide(pushVec);
            }
        }

        private void StateSettled()
        {
            if (_stateMachine.IsStateNew())
            {
                CollisionLayer = 0;
                CollisionMask = 0;
                _velocity = Vector2.Zero;
                GlobalPosition = GlobalPosition.Round();
                _animationPlayer.Play(ANIM_IDLE);
                _collisionShape2d.Disabled = true;
                _deathTimer.Start();
            }

            var playerPosition = GetTree().GetFirstNodeInGroup<Player>(Player.GROUP)?.GlobalPosition ?? Vector2.Zero;
            var near = GlobalPosition.DistanceSquaredTo(playerPosition) < PLAYER_NEAR_DISTANCE * PLAYER_NEAR_DISTANCE;
            if (!_deathTimer.IsStopped() && near)
            {
                _deathTimer.Start();
            }
            else if (near && _blinkAnimationPlayer.IsPlaying())
            {
                ResetBlink();
                _deathTimer.Start();
            }
        }

        private void StatePickedUp()
        {
            if (_stateMachine.IsStateNew())
            {
                _animationPlayer.Play(ANIM_PICKUP);
                _deathTimer.Stop();
                ResetBlink();
                _selectableComponent.Disable();
                GetParent().RemoveChild(this);
                GameZone.FloatersLayer.AddChild(this);
                PlayerInventory.AddItem(this);
            }
            var playerPosition = GetTree().GetFirstNodeInGroup<Player>(Player.GROUP)?.GlobalPosition ?? Vector2.Zero;
            GlobalPosition = playerPosition + new Vector2(0f, PLAYER_HOVER_Y_OFFSET);
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

        private void ResetBlink()
        {
            if (_blinkAnimationPlayer.IsPlaying())
            {
                _blinkAnimationPlayer.Stop(true);
                _blinkAnimationPlayer.Seek(0f, true);
            }
        }

        private void OnDeathTimerTimeout()
        {
            _blinkAnimationPlayer.Play(ANIM_DEFAULT);
        }

        private void OnSelected()
        {
            _stateMachine.ChangeState(StatePickedUp);
        }

        private void OnSelectEnter()
        {
            TooltipUI.ShowItemTooltip(Id);
        }

        private void OnSelectLeave()
        {
            TooltipUI.HideTooltip();
        }
    }
}