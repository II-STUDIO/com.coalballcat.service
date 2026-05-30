using UnityEngine;

namespace Coalballcat.Services
{
    public sealed class MonoPooler<T> : PoolerBase<T>, IMonoPooler where T : PoolableObject
    {
        private T prefab;

        public MonoPooler(T prefab, int initialCapacity = 0, int preSpawnCount = 0,
                          Transform parent = null, bool autoExpand = true)
            : base(initialCapacity, parent, autoExpand)
        {
            if (!prefab) throw new System.NullReferenceException("MonoPooler: prefab is null");
            this.prefab = prefab;

            if (preSpawnCount > 0) PreSpawn(preSpawnCount);
        }

        protected override T CreateInstance(Transform parent)
        {
            T inst = UnityEngine.Object.Instantiate(prefab, parent);
            inst.gameObject.SetActive(false);
            inst.SetPooler(this);
            return inst;
        }

        protected override Transform GetTransform(T item) => item.transform;

        protected override void Activate(T item, Transform parent)
        {
            Transform t = item.transform;
            if (t.parent != parent) t.SetParent(parent);
            item.gameObject.SetActive(true);
            item.OnPoolGet();
        }

        protected override void Deactivate(T item)
        {
            item.OnPoolRelease();
            item.gameObject.SetActive(false);
            Transform t = item.transform;
            if (t.parent != mainParent) t.SetParent(mainParent);
        }

        protected override void DestroyItem(T item) => UnityEngine.Object.Destroy(item.gameObject);

        public void Release(PoolableObject item) => ReleaseCore((T)item);
    }
}
