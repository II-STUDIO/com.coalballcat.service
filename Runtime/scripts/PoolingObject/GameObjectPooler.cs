using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coalballcat.Services
{
    public class GameObjectPooler : IDisposable, IPooler
    {
        private GameObject prefab;
        private Transform mainParent;
        private readonly Queue<GameObject> pool;
        private readonly List<GameObject> contein;
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
            contein = new List<GameObject>(initialCapacity);

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
            contein.Add(instance);
            return instance;
        }

        #region Without_CreateNew_Flag
        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public GameObject Pool() => Pool(mainParent, out bool isCreateNew);

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public GameObject Pool(in Vector3 position)
        {
            GameObject item = Pool(mainParent, out bool isCreateNew);
            item.transform.position = position;
            return item;
        }

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public GameObject Pool(in Vector3 position, in Quaternion rotation)
        {
            GameObject item = Pool(mainParent, out bool isCreateNew);
            item.transform.SetPositionAndRotation(position, rotation);
            return item;
        }

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public GameObject Pool(in Vector3 position, Transform parent)
        {
            GameObject item = Pool(parent, out bool isCreateNew);
            item.transform.position = position;
            return item;
        }

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public GameObject Pool(in Vector3 position, in Quaternion rotation, Transform parent)
        {
            GameObject item = Pool(parent, out bool isCreateNew);
            item.transform.SetPositionAndRotation(position, rotation);
            return item;
        }

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public GameObject Pool(Transform parent) => Pool(parent, out bool isCreateNew);
        #endregion


        #region With_CreateNew_Flag
        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public GameObject Pool(out bool isCreateNew) => Pool(mainParent, out isCreateNew);

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public GameObject Pool(in Vector3 position, out bool isCreateNew)
        {
            GameObject item = Pool(mainParent, out isCreateNew);
            item.transform.position = position;
            return item;
        }

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public GameObject Pool(in Vector3 position, in Quaternion rotation, out bool isCreateNew)
        {
            GameObject item = Pool(mainParent, out isCreateNew);
            item.transform.SetPositionAndRotation(position, rotation);
            return item;
        }

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public GameObject Pool(in Vector3 position, Transform parent, out bool isCreateNew)
        {
            GameObject item = Pool(parent, out isCreateNew);
            item.transform.position = position;
            return item;
        }

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public GameObject Pool(in Vector3 position, in Quaternion rotation, Transform parent, out bool isCreateNew)
        {
            GameObject item = Pool(parent, out isCreateNew);
            item.transform.SetPositionAndRotation(position, rotation);
            return item;
        }

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public GameObject Pool(Transform parent, out bool isCreateNew)
        {
            if (pool.Count == 0)
            {
                isCreateNew = true;

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
                isCreateNew = false;

                GameObject item = pool.Dequeue();
                item.SetActive(true);

                Transform itemTransform = item.transform;
                if (itemTransform.parent != parent)
                    itemTransform.SetParent(parent);

                return item;
            }
        }
        #endregion

        /// <summary>
        /// Return an object to the pool
        /// </summary>
        public void Release(GameObject item)
        {
            if (item == null)
                return;

            if (pool.Contains(item))
                return;

            item.SetActive(false);

            Transform itemTransform = item.transform;
            if (itemTransform.parent != mainParent)
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

                UnityEngine.Object.Destroy(item);
            }

            contein.Clear();
            pool.Clear();
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
}