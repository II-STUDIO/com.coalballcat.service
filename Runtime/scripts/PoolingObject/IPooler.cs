namespace Coalballcat.Services
{
    public interface IPooler
    {
        void DisposeWithoutUninitialize();
    }

    public interface IMonoPooler : IPooler
    {
        void Release(PoolableObject item);
    }
}
