using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Type, object> _channels = new();

    // ── Subscribe ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Subscribe to an event. Returns a token — call Dispose() to unsubscribe.
    /// Use inside GameListener.RegisterListeners() and it's handled automatically.
    /// </summary>
    public static void On<T>(Action<T> listen) where T : struct
    {
        var c = GetOrCreate<T>();
        c.RegisterListener(listen);
    }

    public static void Off<T>(Action<T> listen) where T : struct
    {
        if (_channels.TryGetValue(typeof(T), out var channel))
            ((EventChannel<T>)channel).UnregisterListener(listen);
    }

    // ── Emit ──────────────────────────────────────────────────────────────────

    public static void Emit<T>(T context) where T : struct
    {
        GetOrCreate<T>().Raise(context);
    }

    /// <summary>Emit a signal event (no context needed).</summary>
    public static void Emit<T>() where T : struct
    {
        Emit(default(T));
    }

    // ── Blackboard ────────────────────────────────────────────────────────────

    /// <summary>Store context without raising listeners.</summary>
    public static void Write<T>(T context) where T : struct
    {
        GetOrCreate<T>().Set(context);
    }

    /// <summary>Read last written/emitted context without subscribing.</summary>
    public static T Read<T>() where T : struct
    {
        if (_channels.TryGetValue(typeof(T), out var channel))
            return ((EventChannel<T>)channel).Get();

        return default;
    }


    // ── Internal ──────────────────────────────────────────────────────────────

    private static EventChannel<T> GetOrCreate<T>() where T : struct
    {
        var t = typeof(T);
        if (!_channels.TryGetValue(t, out var channel))
        {
            channel = new EventChannel<T>();
            _channels.Add(t, channel);
        }
        return (EventChannel<T>)channel;
    }

}