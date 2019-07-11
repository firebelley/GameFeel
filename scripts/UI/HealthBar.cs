using GameFeel.GameObject;
using GameFeel.Interface;
using Godot;

public class HealthBar : ProgressBar
{
    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        if (GetOwner() is Spider s)
        {
            s.Connect(nameof(Spider.DamageReceived), this, nameof(OnDamageReceived));
        }
    }

    private void OnDamageReceived(float damage)
    {
        Value = (GetOwner() as IDamageReceiver).GetCurrentHealthPercent();
        _animationPlayer.Stop(true);
        _animationPlayer.Play("bounce");
    }
}