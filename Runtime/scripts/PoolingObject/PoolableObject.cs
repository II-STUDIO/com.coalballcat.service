using UnityEngine;

namespace Coalballcat.Services
{
    /// <summary>
    /// Base class for any MonoBehaviour that lives inside a MonoPooler.
    /// Inherit from this to get self-release and optional pool lifecycle hooks.
    ///
    /// NOTE: Plain GameObjects that have NO pool-specific logic do NOT need this.
    ///       Use GameObjectPooler for those — no MonoBehaviour overhead.
    /// </summary>
    public class PoolableObject : MonoBehaviour
    {
        private IMonoPooler pooler;

        internal void SetPooler(IMonoPooler pooler)
        {
            this.pooler = pooler;
        }

        /// <summary>
        /// Call this to return the object to its pool from anywhere.
        /// </summary>
        public void ReturnPool()
        {
            if (pooler == null)
            {
                Debug.LogWarning($"[PoolableObject] {name} has no pooler assigned. Destroying instead.", this);
                Destroy(gameObject);
                return;
            }
            pooler.Release(this);
        }

        // ──────────────────────────────────────────────
        // Optional lifecycle hooks — override as needed
        // ──────────────────────────────────────────────

        /// <summary>Called right after this object is retrieved from the pool (after SetActive(true)).</summary>
        public virtual void OnPoolGet() { }

        /// <summary>Called right before this object is returned to the pool (before SetActive(false)).</summary>
        public virtual void OnPoolRelease() { }
    }
}
