using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coalballcat.Services
{
    public class GameObjectPoolerGroup : IDisposable
    {
        private readonly Dictionary<GameObject, GameObjectPooler> poolers;
        private Transform parent;
        private int initialCapacity;

        public GameObjectPoolerGroup(int initialCapacity, Transform parent = null)
        {
            poolers = new Dictionary<GameObject, GameObjectPooler>();
            this.parent = parent;
            this.initialCapacity = initialCapacity;
        }

        private GameObjectPooler GetPooler(GameObject prefab)
        {
            if (!poolers.TryGetValue(prefab, out GameObjectPooler pooler))
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

        private GameObjectPooler CreatePooler(GameObject prefab)
        {
            return new GameObjectPooler(prefab, initialCapacity, parent: parent, autoExpand: true);
        }

        #region without_isCreateNew_flag
        public GameObject Pool(GameObject prefab)
        {
            GameObjectPooler pooler = GetPooler(prefab);

            return pooler.Pool();
        }

        public GameObject Pool(GameObject prefab, Transform parent)
        {
            GameObjectPooler pooler = GetPooler(prefab);

            return pooler.Pool(parent);
        }

        public GameObject Pool(GameObject prefab, in Vector3 position)
        {
            GameObjectPooler pooler = GetPooler(prefab);

            return pooler.Pool(position);
        }

        public GameObject Pool(GameObject prefab, in Vector3 position, in Quaternion rotation)
        {
            GameObjectPooler pooler = GetPooler(prefab);

            return pooler.Pool(position, rotation);
        }

        public GameObject Pool(GameObject prefab, in Vector3 position, Transform parent)
        {
            GameObjectPooler pooler = GetPooler(prefab);

            return pooler.Pool(position, parent);
        }

        public GameObject Pool(GameObject prefab, in Vector3 position, in Quaternion rotation, Transform parent)
        {
            GameObjectPooler pooler = GetPooler(prefab);

            return pooler.Pool(position, rotation, parent);
        }
        #endregion


        #region with_isCreateNew_flag
        public GameObject Pool(GameObject prefab, out bool isCreateNew)
        {
            GameObjectPooler pooler = GetPooler(prefab);

            return pooler.Pool(out isCreateNew);
        }

        public GameObject Pool(GameObject prefab, Transform parent, out bool isCreateNew)
        {
            GameObjectPooler pooler = GetPooler(prefab);

            return pooler.Pool(parent, out isCreateNew);
        }

        public GameObject Pool(GameObject prefab, in Vector3 position, out bool isCreateNew)
        {
            GameObjectPooler pooler = GetPooler(prefab);

            return pooler.Pool(position, out isCreateNew);
        }

        public GameObject Pool(GameObject prefab, in Vector3 position, in Quaternion rotation, out bool isCreateNew)
        {
            GameObjectPooler pooler = GetPooler(prefab);

            return pooler.Pool(position, rotation, out isCreateNew);
        }

        public GameObject Pool(GameObject prefab, in Vector3 position, Transform parent, out bool isCreateNew)
        {
            GameObjectPooler pooler = GetPooler(prefab);

            return pooler.Pool(position, parent, out isCreateNew);
        }

        public GameObject Pool(GameObject prefab, in Vector3 position, in Quaternion rotation, Transform parent, out bool isCreateNew)
        {
            GameObjectPooler pooler = GetPooler(prefab);

            return pooler.Pool(position, rotation, parent, out isCreateNew);
        }
        #endregion

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
            parent = null;
        }
    }
}