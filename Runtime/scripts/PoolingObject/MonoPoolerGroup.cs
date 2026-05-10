using UnityEngine;

namespace Coalballcat.Services
{
    /// <summary>
    /// Multi-prefab pool for typed MonoBehaviours extending PoolableObject.
    /// Because each PoolableObject holds a reference to its own pooler,
    /// release always routes correctly — no instance→prefab map needed here.
    /// </summary>
    public sealed class MonoPoolerGroup<T> : PoolerGroupBase<T, MonoPooler<T>>
        where T : PoolableObject
    {
        public MonoPoolerGroup(int initialCapacity, Transform parent = null)
            : base(initialCapacity, parent) { }

        // ── Hook ──────────────────────────────────────────────────────────────

        protected override MonoPooler<T> CreatePooler(T prefab)
            => new MonoPooler<T>(prefab, initialCapacity, parent: parent, autoExpand: true);

        // OnInstancePooled  — not needed, PoolableObject self-routes via SetPooler
        // OnPoolerReleasing — not needed, no side-data to clean up

        // ── Release ──────────────────────────────────────────────────────────

        /// <summary>
        /// Routes back to the object's own pooler via ReturnPool().
        /// No map lookup required — the reference is stored on the object itself.
        /// </summary>
        public void Release(PoolableObject item) => item.ReturnPool();
    }
}
