using NUnit.Framework;
using UnityEngine;

namespace Arikan.MiniMapTests
{
    public class MiniMapMathTests
    {
        private const float Tolerance = 1e-3f;
        private GameObject target;

        [SetUp]
        public void SetUp()
        {
            target = new GameObject("Target");
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(target);
        }

        [Test]
        public void WorldToMap_XZ_MeasuresFromBoundsCenter()
        {
            var worldRect = Rect.MinMaxRect(100f, 50f, 200f, 150f);
            var mapSize = new Vector2(100f, 100f);

            AssertVector(Vector2.zero, MiniMapMath.WorldToMap(new Vector3(150f, 7f, 100f), worldRect, mapSize, MiniMapPlane.XZ));
            AssertVector(new Vector2(50f, 50f), MiniMapMath.WorldToMap(new Vector3(200f, 0f, 150f), worldRect, mapSize, MiniMapPlane.XZ));
        }

        [Test]
        public void WorldToMap_XY_UsesXAndY()
        {
            var worldRect = Rect.MinMaxRect(-10f, -5f, 10f, 5f);

            var result = MiniMapMath.WorldToMap(new Vector3(5f, 2.5f, 99f), worldRect, new Vector2(200f, 100f), MiniMapPlane.XY);

            AssertVector(new Vector2(50f, 25f), result);
        }

        [Test]
        public void WorldToMap_NonUniformScale_ScalesEachAxis()
        {
            var worldRect = Rect.MinMaxRect(-50f, -25f, 50f, 25f);

            var result = MiniMapMath.WorldToMap(new Vector3(20f, 0f, 10f), worldRect, new Vector2(100f, 100f), MiniMapPlane.XZ);

            AssertVector(new Vector2(20f, 20f), result);
        }

        [TestCase(0f, 0f, 0f, 0f)]
        [TestCase(0f, 90f, 0f, -90f)]
        [TestCase(30f, 90f, 0f, -90f)]
        [TestCase(0f, 90f, 30f, -90f)]
        [TestCase(60f, 45f, 20f, -45f)]
        public void GetMapAngle_XZ_UsesHeadingOnly(float x, float y, float z, float expected)
        {
            target.transform.rotation = Quaternion.Euler(x, y, z);

            AssertAngle(expected, MiniMapMath.GetMapAngle(target.transform, MiniMapPlane.XZ));
        }

        [Test]
        public void GetMapAngle_XZ_LookingStraightDown_UsesUpVector()
        {
            target.transform.rotation = Quaternion.Euler(90f, 30f, 0f);

            AssertAngle(-30f, MiniMapMath.GetMapAngle(target.transform, MiniMapPlane.XZ));
        }

        [TestCase(0f, 0f)]
        [TestCase(90f, 90f)]
        [TestCase(-45f, -45f)]
        public void GetMapAngle_XY_UsesZRotation(float z, float expected)
        {
            target.transform.rotation = Quaternion.Euler(0f, 0f, z);

            AssertAngle(expected, MiniMapMath.GetMapAngle(target.transform, MiniMapPlane.XY));
        }

        private static void AssertVector(Vector2 expected, Vector2 actual)
        {
            Assert.AreEqual(expected.x, actual.x, Tolerance, "x");
            Assert.AreEqual(expected.y, actual.y, Tolerance, "y");
        }

        private static void AssertAngle(float expected, float actual)
        {
            Assert.AreEqual(0f, Mathf.DeltaAngle(expected, actual), Tolerance, $"expected {expected} but was {actual}");
        }
    }
}
