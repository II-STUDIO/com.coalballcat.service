using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coalballcat.Services
{
    public static class FastMath
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastAbs(this float x) => x < 0f ? -x : x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FastSign(this float x) => x > 0f ? 1 : (x < 0f ? -1 : 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastClamp(float v, float min, float max)
            => v < min ? min : (v > max ? max : v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastLerp(float a, float b, float t)
            => a + (b - a) * (t < 0f ? 0f : (t > 1f ? 1f : t));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastInvLerp(float a, float b, float v)
            => (v - a) / (b - a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastRemap(float iMin, float iMax, float oMin, float oMax, float v)
            => oMin + ((v - iMin) / (iMax - iMin)) * (oMax - oMin);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastLerpUnclamped(float a, float b, float t)
            => a + (b - a) * t;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(float a, float b, float eps = 0.0001f)
            => Mathf.Abs(a - b) < eps;

        /// <summary>Returns true with the given probability in the 0..1 range.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Chance(float probability)
            => Random.value < probability;

        /// <summary>Returns true <paramref name="percent"/>% of the time (percent in 0..100).</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Chance(int percent)
            => Random.Range(0, 100) < percent;

        /// <summary>Returns true with odds of <paramref name="weight"/> out of <paramref name="total"/>.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Chance(int weight, int total)
            => total > 0 && Random.Range(0, total) < weight;
    }
}