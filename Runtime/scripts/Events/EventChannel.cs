using System;

namespace Coalballcat.Services
{
    public class EventChannel<T> where T : struct
    {
        private event Action<T> m_action;
        private T m_lastContext;

        public void Set(T context)
        {
            m_lastContext = context;
        }

        public T Get()
        {
            return m_lastContext;
        }

        public void Raise(T context)
        {
            m_lastContext = context;
            m_action?.Invoke(context);
        }

        public void RegisterListener(Action<T> listener)
        {
            // Guard against double subscription.
            m_action -= listener;
            m_action += listener;
        }

        public void UnregisterListener(Action<T> listener) => m_action -= listener;

        public void Clear()
        {
            m_action = null;
            m_lastContext = default;
        }
    }
}
