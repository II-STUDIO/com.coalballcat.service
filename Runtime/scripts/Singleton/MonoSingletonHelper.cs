using UnityEngine;

namespace Coalballcat.Services
{
    public static class MonoSingletonHelper
    {
        /// <summary>
        /// Set true while the application is quitting so singleton accessors stop resolving.
        /// Lives here (a non-generic class) because <c>[RuntimeInitializeOnLoadMethod]</c>
        /// cannot be declared inside the generic <see cref="MonoSingleton{T}"/>.
        /// </summary>
        public static bool IsQuitting { get; internal set; }

        // Reset shared state on play-mode start so the flag isn't carried over when
        // entering Play Mode with "Reload Domain" disabled.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetState() => IsQuitting = false;

        /// <summary>
        /// Finds an existing instance of <typeparamref name="T"/> in the scene (including inactive
        /// objects). If none exists, auto-creates one unless the type is marked
        /// <see cref="NonAutoCreateSingletonAttribute"/>, in which case <c>null</c> is returned.
        /// </summary>
        public static T FindOrCreate<T>() where T : MonoBehaviour
        {
            T instance = Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
            if (instance != null)
                return instance;

            string name = typeof(T).Name;

            if (typeof(T).IsDefined(typeof(NonAutoCreateSingletonAttribute), false))
            {
                Debug.LogWarning($"[MonoSingleton] No instance of <{name}> found, and it is marked " +
                                 "[NonAutoCreateSingleton]. Returning null.");
                return null;
            }

            Debug.LogWarning($"[MonoSingleton] No instance of <{name}> found. Auto-creating one.");
            return new GameObject($"{name} (Auto-Created Singleton)").AddComponent<T>();
        }
    }
}
