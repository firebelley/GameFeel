namespace GameFeel.Interface
{
    public interface IDamageImparter
    {
        void RegisterHit(IDamageReceiver damageReceiver);
    }
}