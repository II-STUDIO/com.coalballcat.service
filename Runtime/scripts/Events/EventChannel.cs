using System;

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

        if (m_action == null)
            return;

        m_action(context);
    }

    public void RegisterListener(Action<T> listener)
    {
        m_action -= listener;
        m_action += listener;
    }

    public void UnregisterListener(Action<T> listener) => m_action -= listener;
}