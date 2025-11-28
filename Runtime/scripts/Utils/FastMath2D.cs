using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace Coalballcat.Services
{
    public static class FastMath2D
    {
        // Distance
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastDistance(float2 a, float2 b)
        {
            float2 d = a - b;
            return math.sqrt(math.dot(d, d));
        }

        // Squared distance (no sqrt)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastDistanceSqr(float2 a, float2 b)
        {
            float2 d = a - b;
            return math.dot(d, d);
        }

        // Magnitude
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastMagnitude(float2 v)
            => math.sqrt(math.dot(v, v));

        // Squared magnitude
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastMagnitudeSqr(float2 v)
            => math.dot(v, v);

        // Normalize
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 FastNormalize(float2 v)
        {
            return math.normalize(v);
        }

        // Linear interpolation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 FastLerp(float2 a, float2 b, float t)
            => a + (b - a) * t;

        // Dot product
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastDot(float2 a, float2 b)
            => math.dot(a, b);

        // Direction from -> to (normalized)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 FastDirection(float2 from, float2 to)
        {
            return math.normalizesafe(to - from);
        }

        // Angle between two vectors (degrees)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastAngle(float2 from, float2 to)
        {
            float dot = math.dot(math.normalize(from), math.normalize(to));
            dot = math.clamp(dot, -1f, 1f);
            return math.degrees(math.acos(dot));
        }

        // Signed angle (2D)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastSignedAngle(float2 from, float2 to)
        {
            float angle = FastAngle(from, to);
            float cross = from.x * to.y - from.y * to.x;
            return cross < 0 ? -angle : angle;
        }

        // Clamp magnitude
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 FastClampMagnitude(float2 v, float maxLength)
        {
            float2 normalized = math.normalizesafe(v);
            float magSqr = math.lengthsq(v);

            if (magSqr > maxLength * maxLength)
                return normalized * maxLength;

            return v;
        }

        // Move towards by maxDelta
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 FastMoveTowards(float2 current, float2 target, float maxDistanceDelta)
        {
            float2 diff = target - current;
            float sqrDist = math.lengthsq(diff);

            if (sqrDist == 0 || sqrDist <= maxDistanceDelta * maxDistanceDelta)
                return target;

            float dist = math.sqrt(sqrDist);
            return current + diff / dist * maxDistanceDelta;
        }

        // Clamp per component
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 FastClamp(float2 v, float2 min, float2 max)
            => math.clamp(v, min, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SqrMagnitude(in float2 v) => math.dot(v, v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Magnitude(in float2 v) => math.sqrt(math.dot(v, v));
    }
}
