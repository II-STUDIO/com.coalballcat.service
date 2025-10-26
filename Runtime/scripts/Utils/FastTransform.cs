using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coalballcat.Services
{
    public static class FastTransform
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetX(this Transform t, float x)
        {
            var p = t.position;
            p.x = x;
            t.position = p;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetY(this Transform t, float y)
        {
            var p = t.position;
            p.y = y;
            t.position = p;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetZ(this Transform t, float z)
        {
            var p = t.position;
            p.z = z;
            t.position = p;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetLocal(this Transform t)
        {
            t.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            t.localScale = Vector3.one;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastLookAt(this Transform from, Transform to)
        {
            FastLookAt(from, to.position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastLookAt(this Transform from, in Vector3 to)
        {
            Vector3 dir = FastVector3.FastDirection(from.position, to);
            Quaternion quat = FastQuaternion.FastLookRotation(dir, Vector3.up);
            from.rotation = quat;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastLerpLookAt(this Transform from, Transform to, float t)
        {
            FastLerpLookAt(from, to.position, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastLerpLookAt(this Transform from, in Vector3 to, float t)
        {
            Vector3 dir = FastVector3.FastDirection(from.position, to);
            Quaternion quat = FastQuaternion.FastLookRotation(dir, Vector3.up);
            from.rotation = FastQuaternion.FastLerp(from.rotation, quat, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastSlerpLookAt(this Transform from, Transform to, float t)
        {
            FastSlerpLookAt(from, to.position, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastSlerpLookAt(this Transform from, in Vector3 to, float t)
        {
            Vector3 dir = FastVector3.FastDirection(from.position, to);
            Quaternion quat = FastQuaternion.FastLookRotation(dir, Vector3.up);
            from.rotation = FastQuaternion.FastSlerp(from.rotation, quat, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastLookAt2D(this Transform from, Transform to)
        {
            FastLookAt(from, to.position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastLookAt2D(this Transform from, in Vector2 to)
        {
            Quaternion quat = FastQuaternion.FastLookRotation2D(from.position, to);
            from.rotation = quat;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastLerpLookAt2D(this Transform from, Transform to, float t)
        {
            FastLerpLookAt2D(from, to.position, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastLerpLookAt2D(this Transform from, in Vector2 to, float t)
        {
            Quaternion quat = FastQuaternion.FastLookRotation2D(from.position, to);
            from.rotation = FastQuaternion.FastLerp2D(from.rotation, quat, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastLookTowardAt2D(this Transform from, Transform to, float t)
        {
            FastLookTowardAt2D(from, to.position, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastLookTowardAt2D(this Transform from, in Vector2 to, float t)
        {
            Quaternion quat = FastQuaternion.FastLookRotation2D(from.position, to);
            from.rotation = FastQuaternion.FastRotateTowards2D(from.rotation, quat, t);
        }
    }
}