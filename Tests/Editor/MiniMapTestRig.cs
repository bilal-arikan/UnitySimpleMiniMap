using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Arikan.MiniMapTests
{
    /// <summary>
    /// Builds a minimal mini map hierarchy for edit mode tests and destroys it on dispose.
    /// </summary>
    internal sealed class MiniMapTestRig : IDisposable
    {
        private readonly List<Object> created = new List<Object>();

        public MiniMapBounds Bounds { get; }
        public MiniMapView View { get; }
        public RectTransform Viewport { get; }
        public RectTransform Map { get; }

        public MiniMapTestRig(Vector3 bottomLeft, Vector3 topRight, Vector2 mapSize, MiniMapPlane plane = MiniMapPlane.XZ)
        {
            Bounds = CreateBounds(bottomLeft, topRight, plane);

            var root = CreateRect("MiniMap", null, new Vector2(256f, 256f));
            View = root.gameObject.AddComponent<MiniMapView>();
            Viewport = CreateRect("Viewport", root, new Vector2(200f, 200f));
            Map = CreateRect("Map", Viewport, mapSize);

            var prefab = Track(new GameObject("IconPrefab", typeof(RectTransform), typeof(Image)));
            View.uiDotPrefab = prefab.GetComponent<Image>();
            View.centeredDotCanvas = Viewport;
            View.otherDotCanvas = Map;
            View.miniMapBounds = Bounds;
        }

        public Transform CreateTarget(string name, Vector3 position, Vector3 eulerAngles)
        {
            var target = Track(new GameObject(name)).transform;
            target.SetPositionAndRotation(position, Quaternion.Euler(eulerAngles));
            return target;
        }

        /// <summary>
        /// Where a world point is drawn inside the viewport with the current map layout.
        /// </summary>
        public Vector2 WorldPointInViewport(Vector3 world)
        {
            var mapPoint = Map.rect.center + MiniMapMath.WorldToMap(world, Bounds.GetPlaneRect(), Map.rect.size, Bounds.plane);
            return Viewport.InverseTransformPoint(Map.TransformPoint(mapPoint));
        }

        public T Track<T>(T obj) where T : Object
        {
            created.Add(obj);
            return obj;
        }

        public void Dispose()
        {
            for (var i = created.Count - 1; i >= 0; i--)
            {
                if (created[i] != null)
                {
                    Object.DestroyImmediate(created[i]);
                }
            }
            created.Clear();
        }

        private MiniMapBounds CreateBounds(Vector3 bottomLeft, Vector3 topRight, MiniMapPlane plane)
        {
            var root = Track(new GameObject("Bounds"));
            var bounds = root.AddComponent<MiniMapBounds>();
            bounds.plane = plane;
            bounds.topRight = CreateCorner(root.transform, "Max", topRight);
            bounds.bottomLeft = CreateCorner(root.transform, "Min", bottomLeft);
            return bounds;
        }

        private static Transform CreateCorner(Transform parent, string name, Vector3 position)
        {
            var corner = new GameObject(name).transform;
            corner.SetParent(parent, false);
            corner.position = position;
            return corner;
        }

        private RectTransform CreateRect(string name, Transform parent, Vector2 size)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            if (parent == null)
            {
                Track(gameObject);
            }

            var rect = (RectTransform)gameObject.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.localPosition = Vector3.zero;
            return rect;
        }
    }
}
