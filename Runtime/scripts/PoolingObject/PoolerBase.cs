using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coalballcat.Services
{
    public abstract class PoolerBase<T> : IDisposable, IPooler where T : UnityEngine.Object
    {
        protected Transform mainParent;
        private readonly Queue<T> pool;
        private readonly List<T> container;
        private readonly HashSet<T> pooledSet;
        private readonly bool autoExpand;

        public int PooledCount => pool.Count;
        public int TotalCount  => container.Count;
        public int ActiveCount => container.Count - pool.Count;

        protected PoolerBase(int initialCapacity, Transform parent, bool autoExpand)
        {
            mainParent      = parent;
            this.autoExpand = autoExpand;
            pool      = new Queue<T>(initialCapacity);
            container = new List<T>(initialCapacity);
            pooledSet = new HashSet<T>(initialCapacity);
            PoolManager.Instance.InitializePooler(this);
        }

        /// <summary>
        /// Call at the END of the derived constructor after all fields are set.
        /// Never call abstract/virtual methods from a base constructor.
        /// </summary>
        protected void PreSpawn(int count)
        {
            for (int i = 0; i < count; i++)
            {
                T inst = CreateInstance(mainParent);
                container.Add(inst);
                pool.Enqueue(inst);
                pooledSet.Add(inst);
            }
        }

        // ── Template methods ──────────────────────────────────────────────────

        protected abstract T         CreateInstance(Transform parent);
        protected abstract Transform GetTransform(T item);
        protected abstract void      Activate(T item, Transform parent);
        protected abstract void      Deactivate(T item);
        protected abstract void      DestroyItem(T item);

        // ── Core ──────────────────────────────────────────────────────────────

        protected T PoolCore(Transform parent)
        {
            if (pool.Count == 0)
            {
                if (!autoExpand)
                    throw new InvalidOperationException(
                        $"{GetType().Name}: pool exhausted and autoExpand is disabled.");

                T newItem = CreateInstance(parent);
                if (!newItem) return null;
                container.Add(newItem);
                Activate(newItem, parent);
                return newItem;
            }

            T item = pool.Dequeue();
            if (!item) return null;     // destroyed externally

            pooledSet.Remove(item);
            Activate(item, parent);
            return item;
        }

        protected void ReleaseCore(T item)
        {
            if (!item) return;
            if (pooledSet.Contains(item)) return;   // double-release guard

            Deactivate(item);
            pool.Enqueue(item);
            pooledSet.Add(item);
        }

        // ── Public overloads ──────────────────────────────────────────────────

        public T Pool()                                                               => PoolCore(mainParent);
        public T Pool(Transform parent)                                               => PoolCore(parent);

        // FIX: guard null before accessing transform — PoolCore can return null
        // if an object was destroyed externally while sitting in the queue.
        public T Pool(in Vector3 position)
        {
            var i = PoolCore(mainParent);
            if (i) GetTransform(i).position = position;
            return i;
        }

        public T Pool(in Vector3 position, in Quaternion rotation)
        {
            var i = PoolCore(mainParent);
            if (i) GetTransform(i).SetPositionAndRotation(position, rotation);
            return i;
        }

        public T Pool(in Vector3 position, Transform parent)
        {
            var i = PoolCore(parent);
            if (i) GetTransform(i).position = position;
            return i;
        }

        public T Pool(in Vector3 position, in Quaternion rotation, Transform parent)
        {
            var i = PoolCore(parent);
            if (i) GetTransform(i).SetPositionAndRotation(position, rotation);
            return i;
        }

        // With isNew flag ──────────────────────────────────────────────────────

        public T Pool(out bool isNew)                                                              => PoolWithFlag(mainParent, out isNew);
        public T Pool(Transform parent, out bool isNew)                                            => PoolWithFlag(parent, out isNew);

        public T Pool(in Vector3 position, out bool isNew)
        {
            var i = PoolWithFlag(mainParent, out isNew);
            if (i) GetTransform(i).position = position;
            return i;
        }

        public T Pool(in Vector3 position, in Quaternion rotation, out bool isNew)
        {
            var i = PoolWithFlag(mainParent, out isNew);
            if (i) GetTransform(i).SetPositionAndRotation(position, rotation);
            return i;
        }

        public T Pool(in Vector3 position, Transform parent, out bool isNew)
        {
            var i = PoolWithFlag(parent, out isNew);
            if (i) GetTransform(i).position = position;
            return i;
        }

        public T Pool(in Vector3 position, in Quaternion rotation, Transform parent, out bool isNew)
        {
            var i = PoolWithFlag(parent, out isNew);
            if (i) GetTransform(i).SetPositionAndRotation(position, rotation);
            return i;
        }

        private T PoolWithFlag(Transform parent, out bool isNew)
        {
            isNew = pool.Count == 0;
            return PoolCore(parent);
        }

        // ── Bulk ─────────────────────────────────────────────────────────────

        public void ReleaseAll()
        {
            int count = container.Count;
            for (int i = 0; i < count; i++)
                ReleaseCore(container[i]);
        }

        // ── Cleanup ───────────────────────────────────────────────────────────

        public void Clear()
        {
            int count = container.Count;
            for (int i = 0; i < count; i++)
                if (container[i]) DestroyItem(container[i]);
            container.Clear();
            pool.Clear();
            pooledSet.Clear();
        }

        // virtual so derived classes can properly override (not hide with 'new')
        public virtual void Dispose()
        {
            Clear();
            mainParent = null;

            // Guard: PoolManager may already be gone during application/scene teardown.
            if (PoolManager.HasInstance)
                PoolManager.Instance.UninitializePooler(this);
        }

        public void DisposeWithoutUninitialize() => Clear();
        void IPooler.DisposeWithoutUninitialize() => DisposeWithoutUninitialize();
    }
}
