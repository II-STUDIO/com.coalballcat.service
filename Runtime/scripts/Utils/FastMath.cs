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
            => a + (b - a) * t;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Chance(float probability)
            => Random.value <= probability;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Chane(int probalility)
            => Random.Range(0, 101) <= probalility;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Chane(int probalility, int totalChane)
            => Random.Range(0, totalChane + 1) <= probalility;
    }
}