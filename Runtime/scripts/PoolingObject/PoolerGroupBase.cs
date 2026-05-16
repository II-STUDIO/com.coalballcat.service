using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coalballcat.Services
{
    /// <summary>
    /// All shared group mechanics live here.
    /// Manages a dictionary of poolers keyed by prefab and exposes
    /// all Pool() overloads by delegating to the inner PoolerBase&lt;T&gt;.
    ///
    /// Derived classes implement only what's unique to them:
    ///   - GameObjectPoolerGroup : instance→prefab tracking + TryRelease
    ///   - MonoPoolerGroup&lt;T&gt;   : Release via ReturnPool() — no map needed
    /// </summary>
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

        // ── Abstract / virtual hooks ──────────────────────────────────────────

        /// <summary>Create the concrete pooler type for this group.</summary>
        protected abstract TPooler CreatePooler(T prefab);

        /// <summary>
        /// Called after every successful Pool() call.
        /// GameObjectPoolerGroup uses this to register instance → prefab.
        /// MonoPoolerGroup leaves it empty.
        /// </summary>
        protected virtual void OnInstancePooled(T prefab, T instance) { }

        /// <summary>
        /// Called inside TryReleasePooler before the pooler is disposed.
        /// GameObjectPoolerGroup uses this to remove stale instance records.
        /// </summary>
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

        // ── Pool overloads — written once for both group types ────────────────

        public T Pool(T prefab)
        {
            var i = GetOrCreatePooler(prefab).Pool();
            OnInstancePooled(prefab, i);
            return i;
        }

        public T Pool(T prefab, Transform parent)
        {
            var i = GetOrCreatePooler(prefab).Pool(parent);
            OnInstancePooled(prefab, i);
            return i;
        }

        public T Pool(T prefab, in Vector3 position)
        {
            var i = GetOrCreatePooler(prefab).Pool(position);
            OnInstancePooled(prefab, i);
            return i;
        }

        public T Pool(T prefab, in Vector3 position, in Quaternion rotation)
        {
            var i = GetOrCreatePooler(prefab).Pool(position, rotation);
            OnInstancePooled(prefab, i);
            return i;
        }

        public T Pool(T prefab, in Vector3 position, Transform parent)
        {
            var i = GetOrCreatePooler(prefab).Pool(position, parent);
            OnInstancePooled(prefab, i);
            return i;
        }

        public T Pool(T prefab, in Vector3 position, in Quaternion rotation, Transform parent)
        {
            var i = GetOrCreatePooler(prefab).Pool(position, rotation, parent);
            OnInstancePooled(prefab, i);
            return i;
        }

        // With isNew flag ──────────────────────────────────────────────────────

        public T Pool(T prefab, out bool isNew)
        {
            var i = GetOrCreatePooler(prefab).Pool(out isNew);
            OnInstancePooled(prefab, i);
            return i;
        }

        public T Pool(T prefab, Transform parent, out bool isNew)
        {
            var i = GetOrCreatePooler(prefab).Pool(parent, out isNew);
            OnInstancePooled(prefab, i);
            return i;
        }

        public T Pool(T prefab, in Vector3 position, out bool isNew)
        {
            var i = GetOrCreatePooler(prefab).Pool(position, out isNew);
            OnInstancePooled(prefab, i);
            return i;
        }

        public T Pool(T prefab, in Vector3 position, in Quaternion rotation, out bool isNew)
        {
            var i = GetOrCreatePooler(prefab).Pool(position, rotation, out isNew);
            OnInstancePooled(prefab, i);
            return i;
        }

        public T Pool(T prefab, in Vector3 position, Transform parent, out bool isNew)
        {
            var i = GetOrCreatePooler(prefab).Pool(position, parent, out isNew);
            OnInstancePooled(prefab, i);
            return i;
        }

        public T Pool(T prefab, in Vector3 position, in Quaternion rotation, Transform parent, out bool isNew)
        {
            var i = GetOrCreatePooler(prefab).Pool(position, rotation, parent, out isNew);
            OnInstancePooled(prefab, i);
            return i;
        }

        // ── Pooler management ─────────────────────────────────────────────────

        /// <summary>Dispose the pooler for one prefab and destroy all its instances.</summary>
        public bool TryDisposePooler(T prefab)
        {
            if (!poolers.TryGetValue(prefab, out TPooler pooler)) return false;
            OnPoolerReleasing(prefab);      // let derived class clean up its side-data
            pooler.Dispose();
            poolers.Remove(prefab);
            return true;
        }

        public void ReleaseAll()
        {
            foreach( var pooler in poolers.Values)
            {
                pooler.ReleaseAll();
            }
        }

        // ── Cleanup ───────────────────────────────────────────────────────────

        public void Clear()
        {
            foreach (var pooler in poolers.Values)
                pooler.Clear();
        }

        public void Dispose()
        {
            foreach (var pooler in poolers.Values)
                pooler.Dispose();
            poolers.Clear();
        }
    }
}
