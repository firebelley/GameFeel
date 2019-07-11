namespace GameFeel.Interface
{
    public interface IDamageReceiver
    {
        void DealDamage(float damage);
        float GetCurrentHealthPercent();
    }
}