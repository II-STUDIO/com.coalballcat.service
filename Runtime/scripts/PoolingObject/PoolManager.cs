using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coalballcat.Services
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        private readonly List<IPooler> poolers = new(100);

        public void DestoryAll()
        {
            for(int i = 0; i < poolers.Count; i++)
            {
                var pooler = poolers[i];
                pooler.DisposeWithoutUnitialize();
            }

            poolers.Clear();
        }

        public void InitializePooler(IPooler pooler)
        {
            poolers.AddUnique(pooler);
        }

        public void UnitializePooler(IPooler pooler)
        {
            poolers.FastRemove(pooler);
        }
    }
}