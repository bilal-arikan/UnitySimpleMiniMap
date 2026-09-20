using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace Arikan.MiniMapTests
{
    public class MiniMapBoundsTests
    {
        private const float Tolerance = 1e-4f;
        private GameObject root;

        [TearDown]
        public void TearDown()
        {
            if (root != null)
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void GetPlaneRect_NormalizesSwappedCorners()
        {
            var bounds = CreateBounds(new Vector3(-10f, 0f, -5f), new Vector3(10f, 0f, 5f), MiniMapPlane.XZ);

            var rect = bounds.GetPlaneRect();
            var box = bounds.GetWorldRect();

            Assert.AreEqual(-10f, rect.xMin, Tolerance);
            Assert.AreEqual(-5f, rect.yMin, Tolerance);
            Assert.AreEqual(20f, rect.width, Tolerance);
            Assert.AreEqual(10f, rect.height, Tolerance);
            Assert.AreEqual(20f, box.size.x, Tolerance);
            Assert.AreEqual(10f, box.size.z, Tolerance);
        }

        [Test]
        public void GetPlaneRect_XY_UsesXAndY()
        {
            var bounds = CreateBounds(new Vector3(6f, 8f, -3f), new Vector3(-4f, -2f, 7f), MiniMapPlane.XY);

            var rect = bounds.GetPlaneRect();

            Assert.AreEqual(-4f, rect.xMin, Tolerance);
            Assert.AreEqual(-2f, rect.yMin, Tolerance);
            Assert.AreEqual(10f, rect.width, Tolerance);
            Assert.AreEqual(10f, rect.height, Tolerance);
        }

        [Test]
        public void IsValid_DependsOnAreaOnSelectedPlane()
        {
            var bounds = CreateBounds(new Vector3(10f, 0f, 10f), new Vector3(-10f, 0f, -10f), MiniMapPlane.XY);

            Assert.IsFalse(bounds.IsValid);
            bounds.plane = MiniMapPlane.XZ;
            Assert.IsTrue(bounds.IsValid);
        }

        [Test]
        public void AddComponent_DoesNotMoveGameObject()
        {
            root = new GameObject("Bounds");
            root.transform.position = new Vector3(10f, 0f, 10f);

            root.AddComponent<MiniMapBounds>();

            Assert.AreEqual(new Vector3(10f, 0f, 10f), root.transform.position);
        }

        [Test]
        public void Reset_CreatesChildCornersValidForBothPlanes()
        {
            root = new GameObject("Bounds");
            root.transform.position = new Vector3(10f, 0f, 10f);
            var bounds = root.AddComponent<MiniMapBounds>();

            typeof(MiniMapBounds).GetMethod("Reset", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(bounds, null);

            Assert.AreEqual(new Vector3(10f, 0f, 10f), root.transform.position);
            Assert.AreSame(root.transform, bounds.topRight.parent);
            Assert.AreSame(root.transform, bounds.bottomLeft.parent);
            bounds.plane = MiniMapPlane.XZ;
            Assert.IsTrue(bounds.IsValid);
            bounds.plane = MiniMapPlane.XY;
            Assert.IsTrue(bounds.IsValid);
        }

        private MiniMapBounds CreateBounds(Vector3 topRight, Vector3 bottomLeft, MiniMapPlane plane)
        {
            root = new GameObject("Bounds");
            var bounds = root.AddComponent<MiniMapBounds>();
            bounds.plane = plane;
            bounds.topRight = CreateChild("A", topRight);
            bounds.bottomLeft = CreateChild("B", bottomLeft);
            return bounds;
        }

        private Transform CreateChild(string name, Vector3 position)
        {
            var child = new GameObject(name).transform;
            child.SetParent(root.transform, false);
            child.position = position;
            return child;
        }
    }
}
