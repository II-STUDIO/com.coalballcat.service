namespace Coalballcat.Services
{
    public interface IPooler
    {
        void DisposeWithoutUnitialize();
    }

    public interface IMonoPooler : IPooler
    {
        void Release(PoolableObject item);
    }
}
