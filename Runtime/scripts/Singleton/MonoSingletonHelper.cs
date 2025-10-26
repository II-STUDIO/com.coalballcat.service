using UnityEngine;

namespace Coalballcat.Services
{
    public static class MonoSingletonHelper
    {
        /// <summary>
        /// Find global instance.
        /// </summary>
        public static Inherister FindInstance<Inherister>(Inherister instance) where Inherister : MonoBehaviour
        {
            if (instance)
                return instance;

            Inherister inherister = Object.FindFirstObjectByType<Inherister>(FindObjectsInactive.Include);

            if (inherister == null)
            {
                string name = typeof(Inherister).Name;

                bool isNonAutoCreate = typeof(Inherister).IsDefined(typeof(NonAutoCreateSingletonAttribute), false);
                if (isNonAutoCreate)
                {
                    Debug.LogWarning($"The type of <{name}> not arrive or found");
                    return null;
                }

                Debug.LogWarning($"The type of <{name}> not arrive or found -> auto generate one.");
                return new GameObject(name + " - Singleton (Auto Create)").AddComponent<Inherister>();
            }

            return inherister;
        }

        /// <summary>
        /// Get Instance of mono object.
        /// </summary>
        /// <typeparam name="Inherister"></typeparam>
        /// <param name="gameObject"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static Inherister GetInstance <Inherister>(GameObject gameObject, Inherister instance) where Inherister : MonoBehaviour
        {
            Inherister inheriter = gameObject.GetComponent<Inherister>();

            if (inheriter == null)
                return instance;

            if (instance)
                return instance;

            instance = inheriter;

            return instance;
        }
    }
}