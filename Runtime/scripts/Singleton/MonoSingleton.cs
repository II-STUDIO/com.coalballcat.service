using UnityEngine;

namespace Coalballcat.Services
{
    /// <summary>
    /// Base class for a MonoBehaviour singleton.
    /// Use the self-referencing pattern: <c>class Foo : MonoSingleton&lt;Foo&gt;</c>.
    ///
    /// - Accessing <see cref="Instance"/> finds an existing instance (including inactive),
    ///   or auto-creates one unless the type is marked <see cref="NonAutoCreateSingletonAttribute"/>.
    /// - Duplicate instances are destroyed automatically in <see cref="Awake"/>.
    /// - Override <see cref="Persistent"/> to keep the instance across scene loads.
    /// - Override <see cref="OnSingletonAwake"/> instead of <c>Awake</c> for init logic.
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;

        /// <summary>True if an instance currently exists (does not trigger a find/create).</summary>
        public static bool HasInstance => _instance != null;

        public static T Instance
        {
            get
            {
                if (MonoSingletonHelper.IsQuitting)
                    return _instance;

                // Unity's overloaded null check also treats a destroyed instance as null,
                // so a stale reference left over from a previous play session is re-resolved.
                if (_instance == null)
                    _instance = MonoSingletonHelper.FindOrCreate<T>();

                return _instance;
            }
        }

        /// <summary>Override and return true to apply <c>DontDestroyOnLoad</c> to this singleton.</summary>
        protected virtual bool Persistent => false;

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = (T)this;

            // DontDestroyOnLoad only works on root objects.
            if (Persistent && transform.parent == null)
                DontDestroyOnLoad(gameObject);

            OnSingletonAwake();
        }

        /// <summary>Called once when this object becomes the active singleton instance.</summary>
        protected virtual void OnSingletonAwake() { }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        protected virtual void OnApplicationQuit()
        {
            MonoSingletonHelper.IsQuitting = true;
        }
    }
}
