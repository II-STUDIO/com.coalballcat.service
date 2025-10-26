using UnityEngine;

namespace Coalballcat.Services
{
    public class MonoSingleton<Inherister> : MonoBehaviour where Inherister : MonoBehaviour
    {
        public static Inherister Instance
        {
            get
            {
                if (_isQuiting)
                    return _instance;

                if (!_instance)
                    _instance = MonoSingletonHelper.FindInstance(_instance);

                return _instance;
            }
        }

        private static Inherister _instance;
        private static bool _isQuiting = false;

        protected virtual void Awake()
        {
            SetInstance();
        }

        public void SetInstance()
        {
            if (_instance)
                return;

            _instance = MonoSingletonHelper.GetInstance(gameObject, _instance);
        }

        /// <summary>
        /// Check the status of this instance static object can't check null form Instance property because Instance property away find the validable access can check only this function.
        /// </summary>
        /// <returns>Ture if 'Instance' not be null</returns>
        public static bool IsInstanceValidable
        {
            get => _instance != null;
        }

        private void OnApplicationQuit()
        {
            _isQuiting = true;
        }
    }
}