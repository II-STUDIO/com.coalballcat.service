using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coalballcat.Services
{
    public class MonoPooler<T> : IDisposable, IMonoPooler 
        where T : PoolableObject
    {
        private T prefab;
        private Transform mainParent;
        private readonly Queue<T> pool;
        private readonly List<T> contein;
        private readonly HashSet<T> pooledSet; // track items currently in pool (O(1) checks)
        private readonly bool autoExpand;

        public int Count => pool.Count;

        public MonoPooler(T prefab, int initialCapacity, int preSpawnCount = 0, Transform parent = null, bool autoExpand = true)
        {
            if (!prefab)
                throw new NullReferenceException($"Pooler prefab is null");

            this.prefab = prefab;
            this.mainParent = parent;
            this.autoExpand = autoExpand;
            pool = new Queue<T>(initialCapacity);
            contein = new List<T>(initialCapacity);
            pooledSet = new HashSet<T>(initialCapacity);

            PoolManager.Instance.InitializePooler(this);

            if (preSpawnCount <= 0)
                return;

            for (int i = 0; i < preSpawnCount; i++)
            {
                var inst = CreateInstance(parent);
                pool.Enqueue(inst);
                pooledSet.Add(inst);
            }
        }

        private T CreateInstance(Transform targetParent)
        {
            if (!prefab)
                throw new NullReferenceException($"Pooler prefab is null");

            T instance = UnityEngine.Object.Instantiate(prefab, targetParent);
            instance.gameObject.SetActive(false);
            instance.SetPooler(this);
            contein.Add(instance);
            return instance;
        }

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public T Pool() => Pool(mainParent);

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public T Pool(in Vector3 position)
        {
            T item = Pool(mainParent);
            item.transform.position = position;
            return item;
        }

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public T Pool(in Vector3 position, in Quaternion rotation)
        {
            T item = Pool(mainParent);
            item.transform.SetPositionAndRotation(position, rotation);
            return item;
        }

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public T Pool(in Vector3 position, Transform parent)
        {
            T item = Pool(parent);
            item.transform.position = position;
            return item;
        }

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public T Pool(in Vector3 position, in Quaternion rotation, Transform parent)
        {
            T item = Pool(parent);
            item.transform.SetPositionAndRotation(position, rotation);
            return item;
        }

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public T Pool(Transform parent)
        {
            if (pool.Count == 0)
            {
                if (autoExpand)
                {
                    T item = CreateInstance(parent);
                    if (!item)
                        return null;

                    item.gameObject.SetActive(true);
                    return item;
                }
                else
                {
                    throw new InvalidOperationException($"Pool exhausted and autoExpand of <{prefab.name}> is not allowed!");
                }
            }
            else
            {
                T item = pool.Dequeue();
                if(item == null)
                    return null;

                pooledSet.Remove(item);
                item.gameObject.SetActive(true);

                Transform itemTransform = item.transform;
                if (itemTransform.parent != parent)
                    itemTransform.SetParent(parent);

                return item;
            }
        }

        /// <summary>
        /// Return an object to the pool
        /// </summary>
        public void Release(PoolableObject item)
        {
            if (item == null) 
                return;

            item.gameObject.SetActive(false);

            Transform itemTransform = item.transform;
            if(itemTransform.parent != mainParent)
                itemTransform.SetParent(mainParent);

            var castT = (T)item;
            pool.Enqueue(castT);
            pooledSet.Add(castT);
        }

        /// <summary>
        /// Clear all pooled objects
        /// </summary>
        public void Clear()
        {
            int count = contein.Count;
            for (int i = 0; i < count; i++)
            {
                var item = contein[i];
                if (!item)
                    continue;

                if (!item.gameObject)
                    continue;

                UnityEngine.Object.Destroy(item.gameObject);
            }

            contein.Clear();
            pool.Clear();
            pooledSet.Clear();
        }

        public void Dispose()
        {
            Clear();

            prefab = null;
            mainParent = null;

            PoolManager.Instance.UnitializePooler(this);
        }

        public void DisposeWithoutUnitialize()
        {
            Clear();
        }
    }

    public interface IPooler
    {
        void DisposeWithoutUnitialize();
    }

    public interface IMonoPooler : IPooler
    {
        public void Release(PoolableObject item);
    }
}