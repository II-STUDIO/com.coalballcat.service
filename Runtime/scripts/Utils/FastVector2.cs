using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coalballcat.Services
{
    public static class FastVector2
    {
        // Distance
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastDistance(in Vector2 a, in Vector2 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        // Squared distance (no sqrt)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastDistanceSqr(in Vector2 a, in Vector2 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            return dx * dx + dy * dy;
        }

        // Magnitude
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastMagnitude(in Vector2 v)
            => Mathf.Sqrt(v.x * v.x + v.y * v.y);

        // Squared magnitude
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastMagnitudeSqr(in Vector2 v)
            => v.x * v.x + v.y * v.y;

        // Normalize
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 FastNormalize(in Vector2 v)
        {
            float m = 1.0f / Mathf.Sqrt(v.x * v.x + v.y * v.y);
            return new Vector2(v.x * m, v.y * m);
        }

        // Linear interpolation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 FastLerp(in Vector2 a, in Vector2 b, float t)
            => new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);

        // Dot product
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastDot(in Vector2 a, in Vector2 b)
            => a.x * b.x + a.y * b.y;

        // Direction from 'from' to 'to' (normalized)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 FastDirection(in Vector2 from, in Vector2 to)
        {
            float dx = to.x - from.x;
            float dy = to.y - from.y;
            float invMag = 1.0f / Mathf.Sqrt(dx * dx + dy * dy);
            return new Vector2(dx * invMag, dy * invMag);
        }

        // Angle between two vectors (degrees)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastAngle(in Vector2 from, in Vector2 to)
        {
            float dot = FastDot(FastNormalize(from), FastNormalize(to));
            dot = Mathf.Clamp(dot, -1f, 1f);
            return Mathf.Acos(dot) * Mathf.Rad2Deg;
        }

        // Signed angle from 'from' to 'to' (degrees)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastSignedAngle(in Vector2 from, in Vector2 to)
        {
            float angle = FastAngle(from, to);
            float cross = from.x * to.y - from.y * to.x; // 2D cross product (scalar)
            return cross < 0 ? -angle : angle;
        }

        // Clamp vector magnitude
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 FastClampMagnitude(in Vector2 v, float maxLength)
        {
            float sqrMag = FastMagnitudeSqr(v);
            if (sqrMag > maxLength * maxLength)
            {
                float mag = Mathf.Sqrt(sqrMag);
                float scale = maxLength / mag;
                return new Vector2(v.x * scale, v.y * scale);
            }
            return v;
        }

        // Move towards target by maxDistanceDelta
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 FastMoveTowards(in Vector2 current, in Vector2 target, float maxDistanceDelta)
        {
            float dx = target.x - current.x;
            float dy = target.y - current.y;
            float sqrDist = dx * dx + dy * dy;
            if (sqrDist == 0 || maxDistanceDelta >= Mathf.Sqrt(sqrDist)) return target;
            float dist = Mathf.Sqrt(sqrDist);
            float scale = maxDistanceDelta / dist;
            return new Vector2(current.x + dx * scale, current.y + dy * scale);
        }

        // Clamp per-component
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 FastClamp(in Vector2 v, in Vector2 min, in Vector2 max)
            => new Vector2(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y));
    }
}
