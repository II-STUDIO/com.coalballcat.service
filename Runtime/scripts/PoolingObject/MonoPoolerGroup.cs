using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coalballcat.Services
{
    public class MonoPoolerGroup<T> : IDisposable where T : PoolableObject
    {
        private readonly Dictionary<T, MonoPooler<T>> poolers;
        private readonly Transform parent;
        private int initialCapacity;

        public MonoPoolerGroup(int initialCapacity, Transform parent = null)
        {
            poolers = new Dictionary<T, MonoPooler<T>>();
            this.parent = parent;
            this.initialCapacity = initialCapacity;
        }

        private MonoPooler<T> GetPooler(T prefab)
        {
            if (!poolers.TryGetValue(prefab, out MonoPooler<T> pooler))
            {
                pooler = CreatePooler(prefab);
                poolers.Add(prefab, pooler);
            }

            if (pooler == null)
            {
                pooler = CreatePooler(prefab);
                poolers[prefab] = pooler;
            }

            return pooler;
        }

        private MonoPooler<T> CreatePooler(T prefab)
        {
            return new MonoPooler<T>(prefab, initialCapacity, parent: parent, autoExpand: true);
        }

        #region without_isCreateNew_flag
        public T Pool(T prefab)
        {
            MonoPooler<T> pooler = GetPooler(prefab);
            T instance = pooler.Pool();
            return instance;
        }

        public T Pool(T prefab, Transform parent)
        {
            MonoPooler<T> pooler = GetPooler(prefab);
            T instance = pooler.Pool(parent);
            return instance;
        }

        public T Pool(T prefab, in Vector3 position)
        {
            MonoPooler<T> pooler = GetPooler(prefab);
            T instance = pooler.Pool(position);
            return instance;
        }

        public T Pool(T prefab, in Vector3 position, in Quaternion rotation)
        {
            MonoPooler<T> pooler = GetPooler(prefab);
            T instance = pooler.Pool(position, rotation);
            return instance;
        }

        public T Pool(T prefab, in Vector3 position, Transform parent)
        {
            MonoPooler<T> pooler = GetPooler(prefab);
            T instance = pooler.Pool(position, parent);
            return instance;
        }

        public T Pool(T prefab, in Vector3 position, in Quaternion rotation, Transform parent)
        {
            MonoPooler<T> pooler = GetPooler(prefab);
            T instance = pooler.Pool(position, rotation, parent);
            return pooler.Pool(position, rotation, parent);
        }
        #endregion


        public void Release(PoolableObject pool)
        {
            pool.ReturnPool();
        }

        public bool TryReleasePooler(T prefab)
        {
            if (poolers.TryGetValue(prefab, out MonoPooler<T> pooler))
            {
                pooler.Dispose();
                poolers.Remove(prefab);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Clear()
        {
            var values = poolers.Values;
            foreach (var pooler in values)
            {
                pooler.Clear();
            }
        }

        public void Dispose()
        {
            var values = poolers.Values;
            foreach (var pooler in values)
            {
                pooler.Dispose();
            }

            poolers.Clear();
        }
    }
}