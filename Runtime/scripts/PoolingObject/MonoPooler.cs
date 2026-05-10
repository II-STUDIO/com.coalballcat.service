using UnityEngine;

namespace Coalballcat.Services
{
    /// <summary>
    /// Type-safe pool for MonoBehaviours extending PoolableObject.
    /// Adds SetPooler wiring and OnPoolGet / OnPoolRelease lifecycle hooks.
    /// Use this when the object needs to release itself or react to pool events.
    ///
    /// All pool mechanics are in PoolerBase&lt;T&gt;.
    /// This class only defines what makes a typed component different from a GameObject.
    /// </summary>
    public sealed class MonoPooler<T> : PoolerBase<T>, IMonoPooler where T : PoolableObject
    {
        private T prefab;

        public MonoPooler(T prefab, int initialCapacity, int preSpawnCount = 0,
                          Transform parent = null, bool autoExpand = true)
            : base(initialCapacity, parent, autoExpand)
        {
            if (!prefab) throw new System.NullReferenceException("MonoPooler: prefab is null");
            this.prefab = prefab;

            // PreSpawn AFTER fields are set — never call virtual methods from a base constructor
            if (preSpawnCount > 0) PreSpawn(preSpawnCount);
        }

        protected override T CreateInstance(Transform parent)
        {
            T inst = UnityEngine.Object.Instantiate(prefab, parent);
            inst.gameObject.SetActive(false);
            inst.SetPooler(this);               // wire self-release
            return inst;
        }

        protected override Transform GetTransform(T item) => item.transform;

        protected override void Activate(T item, Transform parent)
        {
            Transform t = item.transform;
            if (t.parent != parent) t.SetParent(parent);
            item.gameObject.SetActive(true);
            item.OnPoolGet();                   // lifecycle hook
        }

        protected override void Deactivate(T item)
        {
            item.OnPoolRelease();               // lifecycle hook before deactivate
            item.gameObject.SetActive(false);
            Transform t = item.transform;
            if (t.parent != mainParent) t.SetParent(mainParent);
        }

        protected override void DestroyItem(T item) => UnityEngine.Object.Destroy(item.gameObject);

        // IMonoPooler — accepts base type so PoolableObject.ReturnPool() can call in
        public void Release(PoolableObject item) => ReleaseCore((T)item);
    }

    // ── Interfaces ────────────────────────────────────────────────────────────

    public interface IPooler
    {
        void DisposeWithoutUnitialize();
    }

    public interface IMonoPooler : IPooler
    {
        void Release(PoolableObject item);
    }
}
