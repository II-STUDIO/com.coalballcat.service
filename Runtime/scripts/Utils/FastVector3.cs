using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coalballcat.Services
{
    public static class FastVector3
    {
        // Distance between two points
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastDistance(in Vector3 a, in Vector3 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            float dz = a.z - b.z;
            return Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        // Squared distance (no sqrt)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastDistanceSqr(in Vector3 a, in Vector3 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            float dz = a.z - b.z;
            return dx * dx + dy * dy + dz * dz;
        }

        // Normalize a vector
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 FastNormalize(in Vector3 v)
        {
            float m = 1.0f / Mathf.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
            return new Vector3(v.x * m, v.y * m, v.z * m);
        }

        // Linear interpolation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 FastLerp(in Vector3 a, in Vector3 b, float t)
            => new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);

        // Dot product
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastDot(in Vector3 a, in Vector3 b)
            => a.x * b.x + a.y * b.y + a.z * b.z;

        // Cross product
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 FastCross(in Vector3 a, in Vector3 b)
            => new Vector3(
                a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x
            );

        // Direction from 'from' to 'to' (normalized)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 FastDirection(in Vector3 from, in Vector3 to)
        {
            float dx = to.x - from.x;
            float dy = to.y - from.y;
            float dz = to.z - from.z;
            float invMag = 1.0f / Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
            return new Vector3(dx * invMag, dy * invMag, dz * invMag);
        }

        // Magnitude
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastMagnitude(in Vector3 v)
            => Mathf.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);

        // Squared magnitude
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastMagnitudeSqr(in Vector3 v)
            => v.x * v.x + v.y * v.y + v.z * v.z;

        // Angle between two vectors (degrees)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastAngle(in Vector3 from, in Vector3 to)
        {
            // Normalize vectors
            float magFrom = 1.0f / Mathf.Sqrt(from.x * from.x + from.y * from.y + from.z * from.z);
            float magTo = 1.0f / Mathf.Sqrt(to.x * to.x + to.y * to.y + to.z * to.z);

            float dot = (from.x * magFrom) * (to.x * magTo)
                      + (from.y * magFrom) * (to.y * magTo)
                      + (from.z * magFrom) * (to.z * magTo);

            dot = Mathf.Clamp(dot, -1f, 1f); // prevent NaN from floating point errors
            return Mathf.Acos(dot) * Mathf.Rad2Deg;
        }


        // Clamp magnitude
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 FastClampMagnitude(in Vector3 v, float maxLength)
        {
            float sqrMag = FastMagnitudeSqr(v);
            if (sqrMag > maxLength * maxLength)
            {
                float mag = Mathf.Sqrt(sqrMag);
                float scale = maxLength / mag;
                return new Vector3(v.x * scale, v.y * scale, v.z * scale);
            }
            return v;
        }

        // Project vector a onto vector b
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 FastProject(in Vector3 a, in Vector3 b)
        {
            float dot = FastDot(a, b);
            float denom = FastDot(b, b);
            float scale = dot / denom;
            return new Vector3(b.x * scale, b.y * scale, b.z * scale);
        }

        // Reflect vector off normal
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 FastReflect(in Vector3 direction, in Vector3 normal)
        {
            float dot = FastDot(direction, normal);
            return new Vector3(
                direction.x - 2f * dot * normal.x,
                direction.y - 2f * dot * normal.y,
                direction.z - 2f * dot * normal.z
            );
        }

        // Linear move towards target by maxDistance
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 FastMoveTowards(in Vector3 current, in Vector3 target, float maxDistanceDelta)
        {
            Vector3 toVector = new Vector3(target.x - current.x, target.y - current.y, target.z - current.z);
            float sqrDist = FastDot(toVector, toVector);
            if (sqrDist == 0 || maxDistanceDelta >= Mathf.Sqrt(sqrDist)) return target;
            float dist = Mathf.Sqrt(sqrDist);
            float scale = maxDistanceDelta / dist;
            return new Vector3(current.x + toVector.x * scale, current.y + toVector.y * scale, current.z + toVector.z * scale);
        }

        // Clamp vector to min/max per component
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 FastClamp(in Vector3 v, in Vector3 min, in Vector3 max)
            => new Vector3(
                Mathf.Clamp(v.x, min.x, max.x),
                Mathf.Clamp(v.y, min.y, max.y),
                Mathf.Clamp(v.z, min.z, max.z)
            );
    }
}
