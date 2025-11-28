using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace Coalballcat.Services
{
    public static class FastMath3D
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(in float3 a, in float3 b)
        {
            return math.sqrt(DistanceSq(a, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceSq(in float3 a, in float3 b)
        {
            float3 d = a - b;
            return math.dot(d, d);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Normalize(in float3 v)
        {
            return math.normalize(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(in float3 a, in float3 b)
        {
            return math.dot(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Direction(in float3 from, in float3 to)
        {
            return math.normalize(to - from);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Lerp(in float3 a, in float3 b, float t)
        {
            return math.lerp(a, b, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 ClampMagnitude(in float3 v, float maxLength)
        {
            float sqrMag = math.lengthsq(v);
            float maxSq = maxLength * maxLength;
            if (sqrMag > maxSq)
            {
                float scale = maxLength / math.sqrt(sqrMag);
                return v * scale;
            }
            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 MoveTowards(in float3 current, in float3 target, float maxDelta)
        {
            float3 dist = target - current;
            float sqrDist = math.lengthsq(dist);
            if (sqrDist == 0f || maxDelta * maxDelta >= sqrDist)
                return target;

            float invDist = 1f / math.sqrt(sqrDist);
            return current + dist * (maxDelta * invDist);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Reflect(in float3 direction, in float3 normal)
        {
            return direction - 2f * math.dot(direction, normal) * normal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SqrMagnitude(in float3 v) => math.dot(v, v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Magnitude(in float3 v) => math.sqrt(math.dot(v, v));
    }
}