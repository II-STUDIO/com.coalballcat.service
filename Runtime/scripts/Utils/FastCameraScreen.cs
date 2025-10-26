using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coalballcat.Services
{
    public class FastCameraScreen
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ScreenCenter()
             => new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWorldVisible(Camera cam, in Vector3 worldPos)
        {
            Vector3 viewPos = cam.WorldToViewportPoint(worldPos);
            return viewPos.z > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsScreenPointVisible(in Vector2 screenPos)
            => screenPos.x >= 0 && screenPos.x <= Screen.width && screenPos.y >= 0 && screenPos.y <= Screen.height;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ScreenToWorld(Camera cam, in Vector2 screenPos, float distance = 10f)
            => cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, distance));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RaycastMouseToWorldPlanePosition(Camera cam, out Vector3 target)
        {
            return RaycastScreenToWorldPlanePosition(cam, Input.mousePosition, out target);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RaycastScreenToWorldPlanePosition(Camera cam, in Vector2 screenPos, out Vector3 target)
        {
            Ray ray = cam.ScreenPointToRay(screenPos);

            Plane plane = new Plane(Vector3.up, Vector3.zero);

            if (plane.Raycast(ray, out float distance))
            {
                target = ray.GetPoint(distance);
                return true;
            }
            else
            {
                target = Vector3.zero;
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RaycastMousePosition2D(Camera cam, out RaycastHit2D hit, int layerMask = Physics2D.DefaultRaycastLayers)
        {
            return Raycast2D(cam, Input.mousePosition, out hit, layerMask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Raycast2D(Camera cam, in Vector2 screenPos, out RaycastHit2D hit, int layerMask = Physics2D.DefaultRaycastLayers)
        {
            Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);
            hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, layerMask);
            return hit.collider != null;
        }
    }
}