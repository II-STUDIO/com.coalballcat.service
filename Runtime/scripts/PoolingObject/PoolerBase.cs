using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coalballcat.Services
{
    /// <summary>
    /// All shared pool mechanics live here — Queue, HashSet, List, overloads, PoolManager wiring.
    /// Derived classes only implement what makes them different (4 abstract methods).
    /// </summary>
    public abstract class PoolerBase<T> : IDisposable, IPooler where T : UnityEngine.Object
    {
        protected Transform mainParent;
        private readonly Queue<T> pool;
        private readonly List<T> container;
        private readonly HashSet<T> pooledSet;
        private readonly bool autoExpand;

        public int PooledCount => pool.Count;
        public int TotalCount  => container.Count;

        // ── Constructor ───────────────────────────────────────────────────────

        protected PoolerBase(int initialCapacity, Transform parent, bool autoExpand)
        {
            mainParent    = parent;
            this.autoExpand = autoExpand;
            pool      = new Queue<T>(initialCapacity);
            container = new List<T>(initialCapacity);
            pooledSet = new HashSet<T>(initialCapacity);
            PoolManager.Instance.InitializePooler(this);
        }

        /// <summary>
        /// Call this at the END of the derived constructor (after all fields are set),
        /// so CreateInstance() can safely access derived-class fields like 'prefab'.
        /// Do NOT call virtual methods from a base constructor.
        /// </summary>
        protected void PreSpawn(int count)
        {
            for (int i = 0; i < count; i++)
            {
                T inst = CreateInstance(mainParent);
                pool.Enqueue(inst);
                pooledSet.Add(inst);
            }
        }

        // ── Template methods — derived classes define these ───────────────────

        protected abstract T         CreateInstance(Transform parent);
        protected abstract Transform GetTransform(T item);
        protected abstract void      Activate(T item, Transform parent);
        protected abstract void      Deactivate(T item);
        protected abstract void      DestroyItem(T item);

        // ── Core logic — one implementation for both poolers ─────────────────

        protected T PoolCore(Transform parent)
        {
            if (pool.Count == 0)
            {
                if (!autoExpand)
                    throw new InvalidOperationException(
                        $"{GetType().Name}: pool exhausted and autoExpand is disabled.");

                T newItem = CreateInstance(parent);
                if (newItem == null) return null;
                Activate(newItem, parent);
                return newItem;
            }

            T item = pool.Dequeue();
            if (item == null) return null;      // destroyed externally — caller should retry or handle

            pooledSet.Remove(item);
            Activate(item, parent);
            return item;
        }

        protected void ReleaseCore(T item)
        {
            if (item == null) return;
            if (pooledSet.Contains(item)) return;   // double-release guard

            Deactivate(item);
            pool.Enqueue(item);
            pooledSet.Add(item);
        }

        // ── Public overloads — defined once for all poolers ──────────────────

        public T Pool()                                                               => PoolCore(mainParent);
        public T Pool(Transform parent)                                               => PoolCore(parent);
        public T Pool(in Vector3 position)                                            { var i = PoolCore(mainParent); GetTransform(i).position = position; return i; }
        public T Pool(in Vector3 position, in Quaternion rotation)                    { var i = PoolCore(mainParent); GetTransform(i).SetPositionAndRotation(position, rotation); return i; }
        public T Pool(in Vector3 position, Transform parent)                          { var i = PoolCore(parent);     GetTransform(i).position = position; return i; }
        public T Pool(in Vector3 position, in Quaternion rotation, Transform parent)  { var i = PoolCore(parent);     GetTransform(i).SetPositionAndRotation(position, rotation); return i; }

        // With isNew flag (useful in group poolers to track new instances)
        public T Pool(out bool isNew)                                                              => PoolWithFlag(mainParent, out isNew);
        public T Pool(Transform parent, out bool isNew)                                            => PoolWithFlag(parent, out isNew);
        public T Pool(in Vector3 position, out bool isNew)                                         { var i = PoolWithFlag(mainParent, out isNew); GetTransform(i).position = position; return i; }
        public T Pool(in Vector3 position, in Quaternion rotation, out bool isNew)                 { var i = PoolWithFlag(mainParent, out isNew); GetTransform(i).SetPositionAndRotation(position, rotation); return i; }
        public T Pool(in Vector3 position, Transform parent, out bool isNew)                       { var i = PoolWithFlag(parent, out isNew);     GetTransform(i).position = position; return i; }
        public T Pool(in Vector3 position, in Quaternion rotation, Transform parent, out bool isNew){ var i = PoolWithFlag(parent, out isNew);     GetTransform(i).SetPositionAndRotation(position, rotation); return i; }

        private T PoolWithFlag(Transform parent, out bool isNew)
        {
            isNew = pool.Count == 0;
            return PoolCore(parent);
        }

        // ── Cleanup ──────────────────────────────────────────────────────────

        public void Clear()
        {
            int count = container.Count;
            for (int i = 0; i < count; i++)
                if (container[i]) DestroyItem(container[i]);
            container.Clear();
            pool.Clear();
            pooledSet.Clear();
        }

        public void Dispose()
        {
            Clear();
            mainParent = null;
            PoolManager.Instance.UninitializePooler(this);
        }

        public void DisposeWithoutUninitialize() => Clear();
        void IPooler.DisposeWithoutUnitialize()  => DisposeWithoutUninitialize();
    }
}
