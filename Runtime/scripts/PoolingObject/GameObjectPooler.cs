using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coalballcat.Services
{
    public class GameObjectPooler : IDisposable, IPooler
    {
        private readonly GameObject prefab;
        private readonly Transform mainParent;
        private readonly Queue<GameObject> pool;
        private readonly bool autoExpand;

        public int Count => pool.Count;

        public GameObjectPooler(GameObject prefab, int initialCapacity, int preSpawnCount = 0, Transform parent = null, bool autoExpand = true)
        {
            if (!prefab)
                throw new NullReferenceException($"Pooler prefab is null");

            this.prefab = prefab;
            this.mainParent = parent;
            this.autoExpand = autoExpand;
            pool = new Queue<GameObject>(initialCapacity);

            PoolManager.Instance.InitializePooler(this);

            if (preSpawnCount <= 0)
                return;

            for (int i = 0; i < initialCapacity; i++)
            {
                pool.Enqueue(CreateInstance(parent));
            }
        }

        private GameObject CreateInstance(Transform targetParent)
        {
            if (!prefab)
                throw new NullReferenceException($"Pooler prefab is null");

            GameObject instance = UnityEngine.Object.Instantiate(prefab, targetParent);
            instance.SetActive(false);
            return instance;
        }

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public GameObject Pool() => Pool(mainParent);

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public GameObject Pool(in Vector3 position)
        {
            GameObject item = Pool(mainParent);
            item.transform.position = position;
            return item;
        }

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public GameObject Pool(in Vector3 position, in Quaternion rotation)
        {
            GameObject item = Pool(mainParent);
            item.transform.SetPositionAndRotation(position, rotation);
            return item;
        }

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public GameObject Pool(in Vector3 position, Transform parent)
        {
            GameObject item = Pool(parent);
            item.transform.position = position;
            return item;
        }

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public GameObject Pool(in Vector3 position, in Quaternion rotation, Transform parent)
        {
            GameObject item = Pool(parent);
            item.transform.SetPositionAndRotation(position, rotation);
            return item;
        }

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public GameObject Pool(Transform parent)
        {
            if (pool.Count == 0)
            {
                if (autoExpand)
                {
                    GameObject item = CreateInstance(parent);
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
                GameObject item = pool.Dequeue();
                item.SetActive(true);
                return item;
            }
        }

        /// <summary>
        /// Return an object to the pool
        /// </summary>
        public void Release(GameObject item)
        {
            if (item == null)
                return;

            item.SetActive(false);
            pool.Enqueue(item);
        }

        /// <summary>
        /// Clear all pooled objects
        /// </summary>
        public void Clear()
        {
            while (pool.Count > 0)
            {
                var item = pool.Dequeue();
                if (!item)
                    continue;

                UnityEngine.Object.Destroy(item);
            }

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
}