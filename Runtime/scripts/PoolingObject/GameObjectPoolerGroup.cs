using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coalballcat.Services
{
    public class GameObjectPoolerGroup : IDisposable
    {
        private readonly Dictionary<GameObject, GameObjectPooler> poolers;
        private readonly Dictionary<GameObject, GameObject> prefabConnection;
        private readonly List<GameObject> instanceTemp;
        private readonly Transform parent;
        private int initialCapacity;

        public GameObjectPoolerGroup(int initialCapacity, Transform parent = null)
        {
            poolers = new Dictionary<GameObject, GameObjectPooler>();
            prefabConnection = new Dictionary<GameObject, GameObject>();
            instanceTemp = new List<GameObject>(256);
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
            GameObject instance = pooler.Pool();
            prefabConnection[instance] = prefab;
            return instance;
        }

        public GameObject Pool(GameObject prefab, Transform parent)
        {
            GameObjectPooler pooler = GetPooler(prefab);
            GameObject instance = pooler.Pool(parent);
            prefabConnection[instance] = prefab;
            return instance;
        }

        public GameObject Pool(GameObject prefab, in Vector3 position)
        {
            GameObjectPooler pooler = GetPooler(prefab);
            GameObject instance = pooler.Pool(position);
            prefabConnection[instance] = prefab;
            return instance;
        }

        public GameObject Pool(GameObject prefab, in Vector3 position, in Quaternion rotation)
        {
            GameObjectPooler pooler = GetPooler(prefab);
            GameObject instance = pooler.Pool(position, rotation);
            prefabConnection[instance] = prefab;
            return instance;
        }

        public GameObject Pool(GameObject prefab, in Vector3 position, Transform parent)
        {
            GameObjectPooler pooler = GetPooler(prefab);
            GameObject instance = pooler.Pool(position, parent);
            prefabConnection[instance] = prefab;
            return instance;
        }

        public GameObject Pool(GameObject prefab, in Vector3 position, in Quaternion rotation, Transform parent)
        {
            GameObjectPooler pooler = GetPooler(prefab);
            GameObject instance = pooler.Pool(position,rotation,parent);
            prefabConnection[instance] = prefab;
            return instance;
        }
        #endregion


        #region with_isCreateNew_flag
        public GameObject Pool(GameObject prefab, out bool isCreateNew)
        {
            GameObjectPooler pooler = GetPooler(prefab);
            GameObject instance = pooler.Pool(out isCreateNew);
            prefabConnection[instance] = prefab;
            return instance;
        }

        public GameObject Pool(GameObject prefab, Transform parent, out bool isCreateNew)
        {
            GameObjectPooler pooler = GetPooler(prefab);
            GameObject instance = pooler.Pool(parent,out isCreateNew);
            prefabConnection[instance] = prefab;
            return instance;
        }

        public GameObject Pool(GameObject prefab, in Vector3 position, out bool isCreateNew)
        {
            GameObjectPooler pooler = GetPooler(prefab);
            GameObject instance = pooler.Pool(position,out isCreateNew);
            prefabConnection[instance] = prefab;
            return instance;
        }

        public GameObject Pool(GameObject prefab, in Vector3 position, in Quaternion rotation, out bool isCreateNew)
        {
            GameObjectPooler pooler = GetPooler(prefab);
            GameObject instance = pooler.Pool(position, rotation, out isCreateNew);
            prefabConnection[instance] = prefab;
            return instance;
        }

        public GameObject Pool(GameObject prefab, in Vector3 position, Transform parent, out bool isCreateNew)
        {
            GameObjectPooler pooler = GetPooler(prefab);
            GameObject instance = pooler.Pool(position, parent, out isCreateNew);
            prefabConnection[instance] = prefab;
            return instance;
        }

        public GameObject Pool(GameObject prefab, in Vector3 position, in Quaternion rotation, Transform parent, out bool isCreateNew)
        {
            GameObjectPooler pooler = GetPooler(prefab);
            GameObject instance = pooler.Pool(position, rotation, parent, out isCreateNew);
            prefabConnection[instance] = prefab;
            return instance;
        }
        #endregion

        public bool TryRelease(GameObject pool)
        {
            if(prefabConnection.TryGetValue(pool, out GameObject prefab))
            {
                GameObjectPooler pooler = GetPooler(prefab);
                pooler.Release(pool);
                prefabConnection.Remove(pool);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryReleasePooler(GameObject prefab)
        {
            if(poolers.TryGetValue(prefab, out GameObjectPooler pooler))
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

        private void ReleasePrefabConnect(GameObject prefab)
        {
            foreach (var kv in prefabConnection)
            {
                if(kv.Value == prefab)
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
                GameObject instance = instanceTemp[i];
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

            instanceTemp.Clear();
        }

        public void Dispose()
        {
            var values = poolers.Values;
            foreach (var pooler in values)
            {
                pooler.Dispose();
            }

            poolers.Clear();
            prefabConnection.Clear();
            instanceTemp.Clear();
        }
    }
}