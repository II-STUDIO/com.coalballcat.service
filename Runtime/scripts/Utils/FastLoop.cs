using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Coalballcat.Services
{
    public static class FastLoop
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastForEach<T>(int count, Func<int, T> action)
        {
            for (int i = 0; i < count; i++)
                action(i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastForEach(int count, Action<int> action)
        {
            for (int i = 0; i < count; i++)
                action(i);
        }

        //List
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastForEach<T>(this List<T> list, ForAction<T> action)
        {
            for (int i = 0, count = list.Count; i < count; i++)
                action(list[i]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastForEach<T>(this List<T> list, ForActionWithIndex<T> action)
        {
            for (int i = 0, count = list.Count; i < count; i++)
                action(i, list[i]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastForEach<T>(this List<T> list, ForActionBreakable<T> action)
        {
            bool shouldBreak = false;
            for (int i = 0, count = list.Count; i < count && !shouldBreak; i++)
                action(list[i], ref shouldBreak);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastForEach<T>(this List<T> list, ForActionWithIndexBreakable<T> action)
        {
            bool shouldBreak = false;
            for (int i = 0, count = list.Count; i < count && !shouldBreak; i++)
                action(i, list[i], ref shouldBreak);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FastFindIndex<T>(this List<T> list, Func<T, bool> condition)
        {
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                if (condition(list[i]))
                    return i;
            }
            return -1; // Not found
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T FastFind<T>(this List<T> list, Func<T, bool> condition)
        {
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                var entity = list[i];
                if (condition(entity))
                    return entity;
            }
            return default; // Not found
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastContains<T>(this List<T> list, Func<T, bool> condition)
        {
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                if (condition(list[i]))
                    return true;
            }
            return false;
        }


        //Array
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastForEach<T>(this T[] array, ForAction<T> action)
        {
            for (int i = 0, count = array.Length; i < count; i++)
                action(array[i]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastForEach<T>(this T[] array, ForActionWithIndex<T> action)
        {
            for (int i = 0, count = array.Length; i < count; i++)
                action(i, array[i]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastForEach<T>(this T[] array, ForActionBreakable<T> action)
        {
            bool shouldBreak = false;
            for (int i = 0, count = array.Length; i < count && !shouldBreak; i++)
                action(array[i], ref shouldBreak);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastForEach<T>(this T[] array, ForActionWithIndexBreakable<T> action)
        {
            bool shouldBreak = false;
            for (int i = 0, count = array.Length; i < count && !shouldBreak; i++)
                action(i, array[i], ref shouldBreak);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FastFindIndex<T>(this T[] array, Func<T, bool> condition)
        {
            int count = array.Length;
            for (int i = 0; i < count; i++)
            {
                if (condition(array[i]))
                    return i;
            }
            return -1; // Not found
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T FastFind<T>(this T[] array, Func<T, bool> condition)
        {
            int count = array.Length;
            for (int i = 0; i < count; i++)
            {
                var entity = array[i];
                if (condition(entity))
                    return entity;
            }
            return default; // Not found
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastContains<T>(this T[] array, Func<T, bool> condition)
        {
            int count = array.Length;
            for (int i = 0; i < count; i++)
            {
                if (condition(array[i]))
                    return true;
            }
            return false;
        }
    }

    public delegate void ForAction<T>(T entity);
    public delegate void ForActionBreakable<T>(T entity, ref bool stop);
    public delegate void ForActionWithIndex<T>(int index, T entity);
    public delegate void ForActionWithIndexBreakable<T>(int index, T entity, ref bool stop);
}