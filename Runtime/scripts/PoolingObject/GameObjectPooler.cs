using UnityEngine;

namespace Coalballcat.Services
{
    /// <summary>
    /// Pool for plain GameObjects with no pool-specific MonoBehaviour logic.
    /// Ideal for particles, decals, audio sources, simple visual objects.
    ///
    /// All pool mechanics are in PoolerBase&lt;GameObject&gt;.
    /// This class only defines what makes a GameObject different from a component.
    /// </summary>
    public sealed class GameObjectPooler : PoolerBase<GameObject>
    {
        private GameObject prefab;

        public GameObjectPooler(GameObject prefab, int initialCapacity = 0, int preSpawnCount = 0,
                                Transform parent = null, bool autoExpand = true)
            : base(initialCapacity, parent, autoExpand)
        {
            if (!prefab) throw new System.NullReferenceException("GameObjectPooler: prefab is null");
            this.prefab = prefab;

            // PreSpawn AFTER fields are set — never call virtual methods from a base constructor
            if (preSpawnCount > 0) PreSpawn(preSpawnCount);
        }

        protected override GameObject CreateInstance(Transform parent)
        {
            var inst = UnityEngine.Object.Instantiate(prefab, parent);
            inst.SetActive(false);
            return inst;
        }

        protected override Transform GetTransform(GameObject item) => item.transform;

        protected override void Activate(GameObject item, Transform parent)
        {
            Transform t = item.transform;
            if (t.parent != parent) t.SetParent(parent);
            item.SetActive(true);
        }

        protected override void Deactivate(GameObject item)
        {
            item.SetActive(false);
            Transform t = item.transform;
            if (t.parent != mainParent) t.SetParent(mainParent);
        }

        protected override void DestroyItem(GameObject item) => UnityEngine.Object.Destroy(item);

        // Release differs only in parameter type between the two poolers
        public void Release(GameObject item) => ReleaseCore(item);
    }
}
