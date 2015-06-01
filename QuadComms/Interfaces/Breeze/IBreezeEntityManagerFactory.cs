using Breeze.Sharp;

namespace QuadComms.Interfaces.Breeze
{
    public interface IBreezeEntityManagerFactory
    {
        EntityManager CreateEntityManager();
    }
}
