using System.Collections.Generic;
using UnityEngine;

namespace Coalballcat.Services
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        private readonly List<IPooler> poolers = new(100);

        /// <summary>Destroy all pooled objects and clear every registered pooler.</summary>
        public void DestroyAll()
        {
            int count = poolers.Count;
            for (int i = 0; i < count; i++)
                poolers[i].DisposeWithoutUnitialize();

            poolers.Clear();
        }

        public void InitializePooler(IPooler pooler)
        {
            poolers.AddUnique(pooler);
        }

        public void UninitializePooler(IPooler pooler)
        {
            poolers.FastRemove(pooler);
        }
    }
}
