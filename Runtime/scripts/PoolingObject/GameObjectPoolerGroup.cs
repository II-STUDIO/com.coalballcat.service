using System.Collections.Generic;
using UnityEngine;

namespace Coalballcat.Services
{
    /// <summary>
    /// Multi-prefab pool for plain GameObjects.
    /// Because a plain GameObject has no self-knowledge of its pool,
    /// this class maintains an instance-ID → prefab map for TryRelease routing.
    /// </summary>
    public sealed class GameObjectPoolerGroup : PoolerGroupBase<GameObject, GameObjectPooler>
    {
        // Maps GetInstanceID() → prefab so TryRelease can find the right pooler.
        private readonly Dictionary<int, GameObject> prefabByInstanceId;
        private readonly List<int> instanceIdTemp;  // scratch list, avoids alloc in RemoveRecords

        public GameObjectPoolerGroup(int initialCapacity, Transform parent = null)
            : base(initialCapacity, parent)
        {
            prefabByInstanceId = new Dictionary<int, GameObject>();
            instanceIdTemp     = new List<int>(initialCapacity * 2);
        }

        // ── Hooks ─────────────────────────────────────────────────────────────

        protected override GameObjectPooler CreatePooler(GameObject prefab)
            => new GameObjectPooler(prefab, initialCapacity, parent: parent, autoExpand: true);

        /// <summary>Register every pooled instance — not just newly created ones.</summary>
        protected override void OnInstancePooled(GameObject prefab, GameObject instance)
            => prefabByInstanceId[instance.GetInstanceID()] = prefab;

        /// <summary>Clean up instance records when a pooler is being disposed.</summary>
        protected override void OnPoolerReleasing(GameObject prefab)
            => RemoveInstanceRecordsForPrefab(prefab);

        // ── Release ──────────────────────────────────────────────────────────

        /// <summary>Return an instance to its pool. Returns false if unknown to this group.</summary>
        public bool TryRelease(GameObject instance)
        {
            int id = instance.GetInstanceID();
            if (!prefabByInstanceId.TryGetValue(id, out GameObject prefab)) return false;
            GetOrCreatePooler(prefab).Release(instance);
            return true;
        }

        /// <summary>Overload that skips GetInstanceID() if you already cached it.</summary>
        public bool TryRelease(GameObject instance, int cachedInstanceId)
        {
            if (!prefabByInstanceId.TryGetValue(cachedInstanceId, out GameObject prefab)) return false;
            GetOrCreatePooler(prefab).Release(instance);
            return true;
        }

        // ── Cleanup override ─────────────────────────────────────────────────

        public new void Dispose()
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
