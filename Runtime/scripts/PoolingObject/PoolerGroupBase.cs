using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coalballcat.Services
{
    public abstract class PoolerGroupBase<T, TPooler> : IDisposable
        where T       : UnityEngine.Object
        where TPooler : PoolerBase<T>
    {
        protected readonly Dictionary<T, TPooler> poolers;
        protected readonly Transform parent;
        protected readonly int initialCapacity;

        protected PoolerGroupBase(int initialCapacity, Transform parent = null)
        {
            this.initialCapacity = initialCapacity;
            this.parent          = parent;
            poolers = new Dictionary<T, TPooler>();
        }

        // ── Hooks ─────────────────────────────────────────────────────────────

        protected abstract TPooler CreatePooler(T prefab);
        protected virtual void OnInstancePooled(T prefab, T instance) { }
        protected virtual void OnPoolerReleasing(T prefab) { }

        // ── Internal ──────────────────────────────────────────────────────────

        protected TPooler GetOrCreatePooler(T prefab)
        {
            if (!poolers.TryGetValue(prefab, out TPooler pooler) || pooler == null)
            {
                pooler = CreatePooler(prefab);
                poolers[prefab] = pooler;
            }
            return pooler;
        }

        // FIX: guard null instance before calling OnInstancePooled.
        // PoolCore can return null if an object was destroyed externally.
        private T PoolAndNotify(T prefab, T instance)
        {
            if (instance) OnInstancePooled(prefab, instance);
            return instance;
        }

        // ── Pool overloads ────────────────────────────────────────────────────

        public T Pool(T prefab)
            => PoolAndNotify(prefab, GetOrCreatePooler(prefab).Pool());

        public T Pool(T prefab, Transform parent)
            => PoolAndNotify(prefab, GetOrCreatePooler(prefab).Pool(parent));

        public T Pool(T prefab, in Vector3 position)
            => PoolAndNotify(prefab, GetOrCreatePooler(prefab).Pool(position));

        public T Pool(T prefab, in Vector3 position, in Quaternion rotation)
            => PoolAndNotify(prefab, GetOrCreatePooler(prefab).Pool(position, rotation));

        public T Pool(T prefab, in Vector3 position, Transform parent)
            => PoolAndNotify(prefab, GetOrCreatePooler(prefab).Pool(position, parent));

        public T Pool(T prefab, in Vector3 position, in Quaternion rotation, Transform parent)
            => PoolAndNotify(prefab, GetOrCreatePooler(prefab).Pool(position, rotation, parent));

        // With isNew flag ──────────────────────────────────────────────────────

        public T Pool(T prefab, out bool isNew)
            => PoolAndNotify(prefab, GetOrCreatePooler(prefab).Pool(out isNew));

        public T Pool(T prefab, Transform parent, out bool isNew)
            => PoolAndNotify(prefab, GetOrCreatePooler(prefab).Pool(parent, out isNew));

        public T Pool(T prefab, in Vector3 position, out bool isNew)
            => PoolAndNotify(prefab, GetOrCreatePooler(prefab).Pool(position, out isNew));

        public T Pool(T prefab, in Vector3 position, in Quaternion rotation, out bool isNew)
            => PoolAndNotify(prefab, GetOrCreatePooler(prefab).Pool(position, rotation, out isNew));

        public T Pool(T prefab, in Vector3 position, Transform parent, out bool isNew)
            => PoolAndNotify(prefab, GetOrCreatePooler(prefab).Pool(position, parent, out isNew));

        public T Pool(T prefab, in Vector3 position, in Quaternion rotation, Transform parent, out bool isNew)
            => PoolAndNotify(prefab, GetOrCreatePooler(prefab).Pool(position, rotation, parent, out isNew));

        // ── Pooler management ─────────────────────────────────────────────────

        public bool TryDisposePooler(T prefab)
        {
            if (!poolers.TryGetValue(prefab, out TPooler pooler)) return false;
            OnPoolerReleasing(prefab);
            pooler.Dispose();
            poolers.Remove(prefab);
            return true;
        }

        public void ReleaseAll()
        {
            foreach (var pooler in poolers.Values)
                pooler.ReleaseAll();
        }

        // ── Cleanup ───────────────────────────────────────────────────────────

        public void Clear()
        {
            foreach (var pooler in poolers.Values)
                pooler.Clear();
        }

        // FIX: virtual so GameObjectPoolerGroup can override (not hide with 'new')
        public virtual void Dispose()
        {
            foreach (var pooler in poolers.Values)
                pooler.Dispose();
            poolers.Clear();
        }
    }
}
