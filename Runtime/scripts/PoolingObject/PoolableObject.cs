using UnityEngine;

namespace Coalballcat.Services
{
    public class PoolableObject : MonoBehaviour
    {
        private IMonoPooler pooler;

        public void SetPooler(IMonoPooler pooler)
        {
            this.pooler = pooler;
        }

        public void ReturnPool()
        {
            pooler.Release(this);
        }
    }
}