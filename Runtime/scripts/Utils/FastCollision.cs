using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace Coalballcat.Services
{
    public static class FastCollision
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SegmentCircleHit(float3 start, float3 end, float3 center, float radius)
        {
            float3 ab = end - start;
            float abDot = math.dot(ab, ab);
            float3 ac = center - start;

            float t = abDot > 0f ? math.dot(ac, ab) / abDot : 0f;
            t = math.clamp(t, 0f, 1f);

            float3 closest = start + t * ab;
            float distSq = math.lengthsq(closest - center);

            return distSq <= radius * radius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SegmentCircleHit(float3 start, float3 end, float3 center, float radius, out float3 hitPoint)
        {
            float3 ab = end - start;
            float abDot = math.dot(ab, ab);
            float3 ac = center - start;

            float t = abDot > 0f ? math.dot(ac, ab) / abDot : 0f;
            t = math.clamp(t, 0f, 1f);

            hitPoint = start + t * ab;
            float distSq = math.lengthsq(hitPoint - center);

            return distSq <= radius * radius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LineIntersectsCircle(float3 start, float3 end, float3 center, float targetRadius, float safeRadius)
        {
            float combinedRadius = targetRadius + safeRadius;
            float combinedRadiusSqr = combinedRadius * combinedRadius;

            float3 ab = end - start;
            float3 ac = center - start;
            float abDot = math.dot(ab, ab);

            float t = abDot > 0f ? math.dot(ac, ab) / abDot : 0f;
            t = math.clamp(t, 0f, 1f);

            float3 closestPoint = start + ab * t;
            float distSqr = math.lengthsq(closestPoint - center);

            return distSqr <= combinedRadiusSqr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LineIntersectsCircle(float3 start, float3 end, float3 center, float targetRadius, float safeRadius, out float3 hitPoint)
        {
            float combinedRadius = targetRadius + safeRadius;
            float combinedRadiusSqr = combinedRadius * combinedRadius;

            float3 ab = end - start;
            float3 ac = center - start;
            float abDot = math.dot(ab, ab);

            float t = abDot > 0f ? math.dot(ac, ab) / abDot : 0f;
            t = math.clamp(t, 0f, 1f);

            hitPoint = start + ab * t;
            float distSqr = math.lengthsq(hitPoint - center);

            return distSqr <= combinedRadiusSqr;
        }
    }
}
