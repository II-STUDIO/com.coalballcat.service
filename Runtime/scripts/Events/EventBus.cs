using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coalballcat.Services
{
    /// <summary>
    /// Global, type-keyed publish/subscribe bus. Event payloads are value-type structs.
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, object> _channels = new();

        // ── Subscribe ───────────────────────────────────────────────────────────

        /// <summary>Subscribe a listener. Subscribing the same delegate twice is a no-op.</summary>
        public static void On<T>(Action<T> listen) where T : struct
            => GetOrCreate<T>().RegisterListener(listen);

        public static void Off<T>(Action<T> listen) where T : struct
        {
            if (_channels.TryGetValue(typeof(T), out var channel))
                ((EventChannel<T>)channel).UnregisterListener(listen);
        }

        // ── Emit ────────────────────────────────────────────────────────────────

        public static void Emit<T>(T context) where T : struct
            => GetOrCreate<T>().Raise(context);

        /// <summary>Emit a signal event (no payload needed).</summary>
        public static void Emit<T>() where T : struct
            => Emit(default(T));

        // ── Blackboard ──────────────────────────────────────────────────────────

        /// <summary>Store a payload without raising listeners.</summary>
        public static void Write<T>(T context) where T : struct
            => GetOrCreate<T>().Set(context);

        /// <summary>Read the last written/emitted payload without subscribing.</summary>
        public static T Read<T>() where T : struct
        {
            if (_channels.TryGetValue(typeof(T), out var channel))
                return ((EventChannel<T>)channel).Get();

            return default;
        }

        // ── Lifecycle ─────────────────────────────────────────────────────────────

        /// <summary>Remove all listeners and stored payloads for a single event type.</summary>
        public static void Clear<T>() where T : struct
        {
            if (_channels.TryGetValue(typeof(T), out var channel))
                ((EventChannel<T>)channel).Clear();
        }

        /// <summary>Remove all listeners and channels for every event type.</summary>
        public static void ClearAll() => _channels.Clear();

        // Reset on play-mode start so listeners from a previous run don't survive when
        // "Reload Domain" is disabled in the Enter Play Mode settings.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => _channels.Clear();

        // ── Internal ──────────────────────────────────────────────────────────────

        private static EventChannel<T> GetOrCreate<T>() where T : struct
        {
            var type = typeof(T);
            if (!_channels.TryGetValue(type, out var channel))
            {
                channel = new EventChannel<T>();
                _channels.Add(type, channel);
            }
            return (EventChannel<T>)channel;
        }
    }
}
