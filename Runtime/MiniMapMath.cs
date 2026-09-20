using UnityEngine;

namespace Arikan
{
    /// <summary>
    /// Pure conversions between world space and mini map space.
    /// </summary>
    public static class MiniMapMath
    {
        private const float DirectionEpsilon = 1e-6f;

        /// <summary>
        /// Projects a world position onto the map plane.
        /// </summary>
        public static Vector2 ToPlane(Vector3 world, MiniMapPlane plane)
        {
            return plane == MiniMapPlane.XY
                ? new Vector2(world.x, world.y)
                : new Vector2(world.x, world.z);
        }

        /// <summary>
        /// Converts a world position to a map position measured from the map rect center.
        /// </summary>
        /// <param name="world">World position to convert.</param>
        /// <param name="worldRect">World area shown by the map, already projected onto the map plane.</param>
        /// <param name="mapSize">Size of the map rect in its local units.</param>
        /// <param name="plane">World plane represented by the map.</param>
        public static Vector2 WorldToMap(Vector3 world, Rect worldRect, Vector2 mapSize, MiniMapPlane plane)
        {
            var point = ToPlane(world, plane);
            return new Vector2(
                (point.x - worldRect.center.x) / worldRect.width * mapSize.x,
                (point.y - worldRect.center.y) / worldRect.height * mapSize.y);
        }

        /// <summary>
        /// Rotation of a target on the map in degrees, counter-clockwise, where 0 means facing map up.
        /// XZ uses the target's forward vector and XY uses its up vector, so pitch and roll are ignored.
        /// </summary>
        public static float GetMapAngle(Transform target, MiniMapPlane plane)
        {
            Vector2 facing;
            if (plane == MiniMapPlane.XY)
            {
                var up = target.up;
                facing = new Vector2(up.x, up.y);
                if (facing.sqrMagnitude < DirectionEpsilon)
                {
                    var forward = target.forward * -Mathf.Sign(up.z);
                    facing = new Vector2(forward.x, forward.y);
                }
            }
            else
            {
                var forward = target.forward;
                facing = new Vector2(forward.x, forward.z);
                if (facing.sqrMagnitude < DirectionEpsilon)
                {
                    var up = target.up * -Mathf.Sign(forward.y);
                    facing = new Vector2(up.x, up.z);
                }
            }

            return facing.sqrMagnitude < DirectionEpsilon
                ? 0f
                : Mathf.Atan2(-facing.x, facing.y) * Mathf.Rad2Deg;
        }
    }
}
