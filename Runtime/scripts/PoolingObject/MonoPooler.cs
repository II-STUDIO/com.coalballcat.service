using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coalballcat.Services
{
    public class MonoPooler<T> : IDisposable, IPooler 
        where T : Component 
    {
        private readonly T prefab;
        private readonly Transform mainParent;
        private readonly Queue<T> pool;
        private readonly List<GameObject> contein;
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
            contein = new List<GameObject>(initialCapacity);

            PoolManager.Instance.InitializePooler(this);

            if (preSpawnCount <= 0)
                return;

            for (int i = 0; i < initialCapacity; i++)
            {
                pool.Enqueue(CreateInstance(parent));
            }
        }

        private T CreateInstance(Transform targetParent)
        {
            if (!prefab)
                throw new NullReferenceException($"Pooler prefab is null");

            T instance = UnityEngine.Object.Instantiate(prefab, targetParent);
            instance.gameObject.SetActive(false);
            contein.Add(instance.gameObject);
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
        public void Release(T item)
        {
            if (item == null) 
                return;

            item.gameObject.SetActive(false);

            Transform itemTransform = item.transform;
            if(itemTransform.parent != mainParent)
                itemTransform.SetParent(mainParent);

            pool.Enqueue(item);
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
        }

        public void Dispose()
        {
            Clear();

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
}