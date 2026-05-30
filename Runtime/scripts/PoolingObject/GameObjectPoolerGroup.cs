using System.Collections.Generic;
using UnityEngine;

namespace Coalballcat.Services
{
    public sealed class GameObjectPoolerGroup : PoolerGroupBase<GameObject, GameObjectPooler>
    {
        private readonly Dictionary<int, GameObject> prefabByInstanceId;
        private readonly List<int> instanceIdTemp;

        public GameObjectPoolerGroup(int initialCapacity, Transform parent = null)
            : base(initialCapacity, parent)
        {
            prefabByInstanceId = new Dictionary<int, GameObject>();
            instanceIdTemp     = new List<int>(initialCapacity * 2);
        }

        // ── Hooks ─────────────────────────────────────────────────────────────

        protected override GameObjectPooler CreatePooler(GameObject prefab)
            => new GameObjectPooler(prefab, initialCapacity, parent: parent, autoExpand: true);

        protected override void OnInstancePooled(GameObject prefab, GameObject instance)
            => prefabByInstanceId[instance.GetInstanceID()] = prefab;

        protected override void OnPoolerReleasing(GameObject prefab)
            => RemoveInstanceRecordsForPrefab(prefab);

        // ── Release ──────────────────────────────────────────────────────────

        public bool TryRelease(GameObject instance)
        {
            int id = instance.GetInstanceID();
            if (!prefabByInstanceId.TryGetValue(id, out GameObject prefab)) return false;
            GetOrCreatePooler(prefab).Release(instance);
            return true;
        }

        public bool TryRelease(GameObject instance, int cachedInstanceId)
        {
            if (!prefabByInstanceId.TryGetValue(cachedInstanceId, out GameObject prefab)) return false;
            GetOrCreatePooler(prefab).Release(instance);
            return true;
        }

        // FIX: override instead of 'new' so this runs correctly via IDisposable or base reference
        public override void Dispose()
        {
            base.Dispose();
            prefabByInstanceId.Clear();
            instanceIdTemp.Clear();
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private void RemoveInstanceRecordsForPrefab(GameObject prefab)
        {
            foreach (var kv in prefabByInstanceId)
                if (kv.Value == prefab) instanceIdTemp.Add(kv.Key);

            int count = instanceIdTemp.Count;
            for (int i = 0; i < count; i++)
                prefabByInstanceId.Remove(instanceIdTemp[i]);

            instanceIdTemp.Clear();
        }
    }
}
