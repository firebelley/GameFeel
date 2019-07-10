namespace GameFeel.Interface
{
    public interface IDamageDealer
    {
        void RegisterHit(IDamageReceiver damageReceiver);
    }
}