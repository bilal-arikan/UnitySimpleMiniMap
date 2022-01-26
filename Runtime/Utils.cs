
namespace Arikan
{
    using UnityEngine;
    public static class Extensions
    {
        public static Vector3 GetPosition(this Matrix4x4 m)
        {
            Vector3 result = default(Vector3);
            result.x = m.m03;
            result.y = m.m13;
            result.z = m.m23;
            return result;
        }

        public static Quaternion GetRotation(this Matrix4x4 m)
        {
            Vector3 forward = default(Vector3);
            forward.x = m.m02;
            forward.y = m.m12;
            forward.z = m.m22;
            Vector3 upwards = default(Vector3);
            upwards.x = m.m01;
            upwards.y = m.m11;
            upwards.z = m.m21;
            return Quaternion.LookRotation(forward, upwards);
        }

        public static Vector2 XZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }
    }
}