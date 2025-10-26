using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coalballcat.Services
{
    public static class FastQuaternion
    {
        // Quaternion multiplication (rotation composition)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion FastMultiply(in Quaternion a, in Quaternion b)
        {
            return new Quaternion(
                a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y,
                a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x,
                a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w,
                a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z
            );
        }

        // Quaternion conjugate (inverse if unit quaternion)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion FastConjugate(in Quaternion q)
        {
            return new Quaternion(-q.x, -q.y, -q.z, q.w);
        }

        // Quaternion magnitude
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastMagnitude(in Quaternion q)
            => Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);

        // Normalize quaternion
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion FastNormalize(in Quaternion q)
        {
            float mag = 1.0f / FastMagnitude(q);
            return new Quaternion(q.x * mag, q.y * mag, q.z * mag, q.w * mag);
        }

        // Quaternion to Euler angles (degrees)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 FastToEuler(in Quaternion q)
        {
            float sinr_cosp = 2 * (q.w * q.x + q.y * q.z);
            float cosr_cosp = 1 - 2 * (q.x * q.x + q.y * q.y);
            float x = Mathf.Atan2(sinr_cosp, cosr_cosp) * Mathf.Rad2Deg;

            float sinp = 2 * (q.w * q.y - q.z * q.x);
            float y;
            if (Mathf.Abs(sinp) >= 1)
                y = Mathf.Sign(sinp) * 90f;
            else
                y = Mathf.Asin(sinp) * Mathf.Rad2Deg;

            float siny_cosp = 2 * (q.w * q.z + q.x * q.y);
            float cosy_cosp = 1 - 2 * (q.y * q.y + q.z * q.z);
            float z = Mathf.Atan2(siny_cosp, cosy_cosp) * Mathf.Rad2Deg;

            return new Vector3(x, y, z);
        }

        // Euler angles (degrees) to quaternion
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion FastFromEuler(in Vector3 euler)
        {
            float xRad = euler.x * Mathf.Deg2Rad * 0.5f;
            float yRad = euler.y * Mathf.Deg2Rad * 0.5f;
            float zRad = euler.z * Mathf.Deg2Rad * 0.5f;

            float cx = Mathf.Cos(xRad);
            float sx = Mathf.Sin(xRad);
            float cy = Mathf.Cos(yRad);
            float sy = Mathf.Sin(yRad);
            float cz = Mathf.Cos(zRad);
            float sz = Mathf.Sin(zRad);

            return new Quaternion(
                sx * cy * cz - cx * sy * sz,
                cx * sy * cz + sx * cy * sz,
                cx * cy * sz - sx * sy * cz,
                cx * cy * cz + sx * sy * sz
            );
        }

        // Quaternion dot product
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastDot(in Quaternion a, in Quaternion b)
            => a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;

        // Angle between two quaternions (degrees)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastAngle(in Quaternion a, in Quaternion b)
        {
            float dot = FastMath.FastClamp(FastDot(FastNormalize(a), FastNormalize(b)), -1f, 1f);
            return Mathf.Acos(dot) * 2f * Mathf.Rad2Deg;
        }

        // Look rotation quaternion (like Quaternion.LookRotation)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion FastLookRotation(in Vector3 forward, in Vector3 up)
        {
            Vector3 f = FastVector3.FastNormalize(forward);
            Vector3 u = FastVector3.FastNormalize(up);
            Vector3 r = FastVector3.FastNormalize(FastVector3.FastCross(u, f));
            u = FastVector3.FastCross(f, r);

            float m00 = r.x; float m01 = r.y; float m02 = r.z;
            float m10 = u.x; float m11 = u.y; float m12 = u.z;
            float m20 = f.x; float m21 = f.y; float m22 = f.z;

            float t = m00 + m11 + m22;
            Quaternion q;
            if (t > 0f)
            {
                float s = Mathf.Sqrt(t + 1f) * 2f;
                q.w = 0.25f * s;
                q.x = (m21 - m12) / s;
                q.y = (m02 - m20) / s;
                q.z = (m10 - m01) / s;
            }
            else if (m00 > m11 && m00 > m22)
            {
                float s = Mathf.Sqrt(1f + m00 - m11 - m22) * 2f;
                q.w = (m21 - m12) / s;
                q.x = 0.25f * s;
                q.y = (m01 + m10) / s;
                q.z = (m02 + m20) / s;
            }
            else if (m11 > m22)
            {
                float s = Mathf.Sqrt(1f + m11 - m00 - m22) * 2f;
                q.w = (m02 - m20) / s;
                q.x = (m01 + m10) / s;
                q.y = 0.25f * s;
                q.z = (m12 + m21) / s;
            }
            else
            {
                float s = Mathf.Sqrt(1f + m22 - m00 - m11) * 2f;
                q.w = (m10 - m01) / s;
                q.x = (m02 + m20) / s;
                q.y = (m12 + m21) / s;
                q.z = 0.25f * s;
            }
            return q;
        }

        // Slerp (spherical linear interpolation)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion FastSlerp(in Quaternion a, in Quaternion b, float t)
        {
            Quaternion bCopy = b;
            float dot = FastDot(a, bCopy);

            if (dot < 0f)
            {
                bCopy = new Quaternion(-bCopy.x, -bCopy.y, -bCopy.z, -bCopy.w);
                dot = -dot;
            }

            const float DOT_THRESHOLD = 0.9995f;
            if (dot > DOT_THRESHOLD)
            {
                Quaternion result = new Quaternion(
                    Mathf.Lerp(a.x, bCopy.x, t),
                    Mathf.Lerp(a.y, bCopy.y, t),
                    Mathf.Lerp(a.z, bCopy.z, t),
                    Mathf.Lerp(a.w, bCopy.w, t)
                );
                return FastNormalize(result);
            }

            float theta_0 = Mathf.Acos(dot);
            float theta = theta_0 * t;
            float sin_theta = Mathf.Sin(theta);
            float sin_theta_0 = Mathf.Sin(theta_0);

            float s0 = Mathf.Cos(theta) - dot * sin_theta / sin_theta_0;
            float s1 = sin_theta / sin_theta_0;

            return new Quaternion(
                (a.x * s0) + (bCopy.x * s1),
                (a.y * s0) + (bCopy.y * s1),
                (a.z * s0) + (bCopy.z * s1),
                (a.w * s0) + (bCopy.w * s1)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion FastLerp(in Quaternion a, in Quaternion b, float t)
        {
            float dot = FastDot(a, b);
            Quaternion bCopy = b;

            // Ensure shortest path
            if (dot < 0f)
            {
                bCopy = new Quaternion(-bCopy.x, -bCopy.y, -bCopy.z, -bCopy.w);
            }

            // Linear interpolation
            Quaternion result = new Quaternion(
                Mathf.Lerp(a.x, bCopy.x, t),
                Mathf.Lerp(a.y, bCopy.y, t),
                Mathf.Lerp(a.z, bCopy.z, t),
                Mathf.Lerp(a.w, bCopy.w, t)
            );

            return FastNormalize(result); // keep unit quaternion
        }


        // Get angle in degrees to face target
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastLookAngle2D(in Vector2 from,in Vector2 to)
        {
            Vector2 dir = to - from;
            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }

        // Get quaternion rotation around Z-axis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion FastLookRotation2D(in Vector2 from, in Vector2 to)
        {
            float angle = FastLookAngle2D(from, to);
            return Quaternion.Euler(0f, 0f, angle);
        }

        // Rotate smoothly using Lerp
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion FastLerp2D(in Quaternion current, in Quaternion target, float t)
        {
            float angle = Mathf.LerpAngle(current.eulerAngles.z, target.eulerAngles.z, t);
            return Quaternion.Euler(0f, 0f, angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion FastLerp2D(float currentAngle, float targetAngle, float t)
        {
            float angle = Mathf.LerpAngle(currentAngle, targetAngle, t);
            return Quaternion.Euler(0f, 0f, angle);
        }

        // Rotate smoothly using MoveTowards
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion FastRotateTowards2D(float currentAngle, float targetAngle, float maxDelta)
        {
            float angle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, maxDelta);
            return Quaternion.Euler(0f, 0f, angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion FastRotateTowards2D(in Quaternion current, in Quaternion target, float maxDelta)
        {
            float angle = Mathf.MoveTowardsAngle(current.eulerAngles.z, target.eulerAngles.z, maxDelta);
            return Quaternion.Euler(0f, 0f, angle);
        }
    }
}
