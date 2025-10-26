using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Coalballcat.Services
{
    public static class FastCollections
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SafeClear<T>(this List<T> list)
        {
            if (list != null && list.Count > 0) list.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SafeClear<Key, Value>(this Dictionary<Key, Value> dict)
        {
            if (dict != null && dict.Count > 0) dict.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddUnique<T>(this List<T> list, T item)
        {
            if (!list.Contains(item)) list.Add(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastRemove<T>(this List<T> list, T item)
        {
            int index = list.IndexOf(item);
            if (index >= 0)
            {
                int last = list.Count - 1;
                list[index] = list[last];
                list.RemoveAt(last);
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastRemoveAt<T>(this List<T> list, int index)
        {
            int last = list.Count - 1;
            list[index] = list[last];
            list.RemoveAt(last);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastForEach<T>(this List<T> list, Action<T> action)
        {
            for (int i = 0, count = list.Count; i < count; i++)
                action(list[i]);
        }
    }
}