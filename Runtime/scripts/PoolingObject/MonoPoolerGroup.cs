using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coalballcat.Services
{
    public class MonoPoolerGroup<T> : IDisposable where T : Component
    {
        private readonly Dictionary<T, MonoPooler<T>> poolers;
        private readonly Dictionary<T, T> prefabConnection;
        private readonly List<T> instanceTemp;
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
            prefabConnection[instance] = prefab;
            return instance;
        }

        public T Pool(T prefab, Transform parent)
        {
            MonoPooler<T> pooler = GetPooler(prefab);
            T instance = pooler.Pool(parent);
            prefabConnection[instance] = prefab;
            return instance;
        }

        public T Pool(T prefab, in Vector3 position)
        {
            MonoPooler<T> pooler = GetPooler(prefab);
            T instance = pooler.Pool(position);
            prefabConnection[instance] = prefab;
            return instance;
        }

        public T Pool(T prefab, in Vector3 position, in Quaternion rotation)
        {
            MonoPooler<T> pooler = GetPooler(prefab);
            T instance = pooler.Pool(position, rotation);
            prefabConnection[instance] = prefab;
            return instance;
        }

        public T Pool(T prefab, in Vector3 position, Transform parent)
        {
            MonoPooler<T> pooler = GetPooler(prefab);
            T instance = pooler.Pool(position, parent);
            prefabConnection[instance] = prefab;
            return instance;
        }

        public T Pool(T prefab, in Vector3 position, in Quaternion rotation, Transform parent)
        {
            MonoPooler<T> pooler = GetPooler(prefab);
            T instance = pooler.Pool(position, rotation, parent);
            prefabConnection[instance] = prefab;
            return pooler.Pool(position, rotation, parent);
        }
        #endregion


        public bool TryRelease(T pool)
        {
            if (prefabConnection.TryGetValue(pool, out T prefab))
            {
                MonoPooler<T> pooler = GetPooler(prefab);
                pooler.Release(pool);
                prefabConnection.Remove(pool);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryReleasePooler(T prefab)
        {
            if (poolers.TryGetValue(prefab, out MonoPooler<T> pooler))
            {
                ReleasePrefabConnect(prefab);

                pooler.Dispose();
                poolers.Remove(prefab);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ReleasePrefabConnect(T prefab)
        {
            foreach (var kv in prefabConnection)
            {
                if (kv.Value == prefab)
                {
                    instanceTemp.Add(kv.Key);
                    break;
                }
            }

            int count = instanceTemp.Count;

            if (count == 0)
                return;

            for (int i = 0; i < count; i++)
            {
                T instance = instanceTemp[i];
                if (!instance)
                    continue;

                prefabConnection.Remove(instance);
            }

            instanceTemp.Clear();
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