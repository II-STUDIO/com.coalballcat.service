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
        private readonly HashSet<GameObject> pooledSet; // track items currently in pool (O(1) checks)
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
            pooledSet = new HashSet<GameObject>(initialCapacity);

            PoolManager.Instance.InitializePooler(this);

            if (preSpawnCount <= 0)
                return;

            // use preSpawnCount (not initialCapacity) to pre-create instances
            for (int i = 0; i < preSpawnCount; i++)
            {
                var inst = CreateInstance(parent);
                pool.Enqueue(inst);
                pooledSet.Add(inst);
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
        public GameObject Pool() => Pool(mainParent, out bool isCreateNew);

        public GameObject Pool(in Vector3 position)
        {
            GameObject item = Pool(mainParent, out bool isCreateNew);
            item.transform.position = position;
            return item;
        }

        public GameObject Pool(in Vector3 position, in Quaternion rotation)
        {
            GameObject item = Pool(mainParent, out bool isCreateNew);
            item.transform.SetPositionAndRotation(position, rotation);
            return item;
        }

        public GameObject Pool(in Vector3 position, Transform parent)
        {
            GameObject item = Pool(parent, out bool isCreateNew);
            item.transform.position = position;
            return item;
        }

        public GameObject Pool(in Vector3 position, in Quaternion rotation, Transform parent)
        {
            GameObject item = Pool(parent, out bool isCreateNew);
            item.transform.SetPositionAndRotation(position, rotation);
            return item;
        }

        public GameObject Pool(Transform parent) => Pool(parent, out bool isCreateNew);
        #endregion


        #region With_CreateNew_Flag
        public GameObject Pool(out bool isCreateNew) => Pool(mainParent, out isCreateNew);

        public GameObject Pool(in Vector3 position, out bool isCreateNew)
        {
            GameObject item = Pool(mainParent, out isCreateNew);
            item.transform.position = position;
            return item;
        }

        public GameObject Pool(in Vector3 position, in Quaternion rotation, out bool isCreateNew)
        {
            GameObject item = Pool(mainParent, out isCreateNew);
            item.transform.SetPositionAndRotation(position, rotation);
            return item;
        }

        public GameObject Pool(in Vector3 position, Transform parent, out bool isCreateNew)
        {
            GameObject item = Pool(parent, out isCreateNew);
            item.transform.position = position;
            return item;
        }

        public GameObject Pool(in Vector3 position, in Quaternion rotation, Transform parent, out bool isCreateNew)
        {
            GameObject item = Pool(parent, out isCreateNew);
            item.transform.SetPositionAndRotation(position, rotation);
            return item;
        }

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
                if(item == null)
                    return null;

                pooledSet.Remove(item); // remove from set when taken out
                item.SetActive(true);

                Transform itemTransform = item.transform;
                if (itemTransform.parent != parent)
                    itemTransform.SetParent(parent);

                return item;
            }
        }
        #endregion

        public void Release(GameObject item)
        {
            if (item == null)
                return;

            // O(1) check with HashSet instead of O(n) Queue.Contains
            if (pooledSet.Contains(item))
                return;

            item.SetActive(false);

            pool.Enqueue(item);
            pooledSet.Add(item);
        }

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
}