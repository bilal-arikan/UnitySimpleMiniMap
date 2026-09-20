using UnityEngine;

namespace Arikan
{
    /// <summary>
    /// Defines the world area shown by the mini map with two corner transforms.
    /// </summary>
    [DisallowMultipleComponent]
    public class MiniMapBounds : MonoBehaviour
    {
        private const string TopRightName = "TopRight";
        private const string BottomLeftName = "BottomLeft";
        private const float MinimumSize = 1e-4f;

        [Tooltip("World plane shown by the map. XZ for top-down 3D, XY for 2D.")]
        public MiniMapPlane plane = MiniMapPlane.XZ;

        [Tooltip("Corner at the right/top edge of the map. Corners are normalized, so swapping them is safe.")]
        public Transform topRight;

        [Tooltip("Corner at the left/bottom edge of the map.")]
        public Transform bottomLeft;

        /// <summary>
        /// True when both corners exist and span a non-empty area on the map plane.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (topRight == null || bottomLeft == null)
                {
                    return false;
                }

                var rect = GetPlaneRect();
                return rect.width > MinimumSize && rect.height > MinimumSize;
            }
        }

        /// <summary>
        /// Axis-aligned world box between the corners. Its size is never negative.
        /// </summary>
        public Bounds GetWorldRect()
        {
            var min = Vector3.Min(bottomLeft.position, topRight.position);
            var max = Vector3.Max(bottomLeft.position, topRight.position);
            return new Bounds((min + max) * 0.5f, max - min);
        }

        /// <summary>
        /// Area between the corners projected onto the map plane.
        /// </summary>
        public Rect GetPlaneRect()
        {
            var a = MiniMapMath.ToPlane(bottomLeft.position, plane);
            var b = MiniMapMath.ToPlane(topRight.position, plane);
            return Rect.MinMaxRect(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
        }

        private void Awake()
        {
            AssignCornersByName();
        }

        private void OnValidate()
        {
            AssignCornersByName();
        }

        private void Reset()
        {
            AssignCornersByName();
            if (topRight == null)
            {
                topRight = CreateCorner(TopRightName, new Vector3(10f, 10f, 10f));
            }
            if (bottomLeft == null)
            {
                bottomLeft = CreateCorner(BottomLeftName, new Vector3(-10f, -10f, -10f));
            }
        }

        private void AssignCornersByName()
        {
            if (topRight == null)
            {
                topRight = transform.Find(TopRightName);
            }
            if (bottomLeft == null)
            {
                bottomLeft = transform.Find(BottomLeftName);
            }
        }

        private Transform CreateCorner(string cornerName, Vector3 localPosition)
        {
            var corner = new GameObject(cornerName).transform;
            corner.SetParent(transform, false);
            corner.localPosition = localPosition;
#if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(corner.gameObject, "Create Mini Map Bounds Corner");
#endif
            return corner;
        }

        private void OnDrawGizmosSelected()
        {
            if (topRight == null || bottomLeft == null)
            {
                return;
            }

            var box = GetWorldRect();
            var min = box.min;
            var max = box.max;
            Vector3 a, b, c, d;
            if (plane == MiniMapPlane.XY)
            {
                var z = box.center.z;
                a = new Vector3(min.x, min.y, z);
                b = new Vector3(max.x, min.y, z);
                c = new Vector3(max.x, max.y, z);
                d = new Vector3(min.x, max.y, z);
            }
            else
            {
                var y = box.center.y;
                a = new Vector3(min.x, y, min.z);
                b = new Vector3(max.x, y, min.z);
                c = new Vector3(max.x, y, max.z);
                d = new Vector3(min.x, y, max.z);
            }

            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, d);
            Gizmos.DrawLine(d, a);
        }
    }
}
