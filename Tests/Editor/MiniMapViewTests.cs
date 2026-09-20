using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Arikan.MiniMapTests
{
    public class MiniMapViewTests
    {
        private const float Tolerance = 1e-3f;
        private MiniMapTestRig rig;

        [TearDown]
        public void TearDown()
        {
            rig?.Dispose();
            rig = null;
        }

        [Test]
        public void Follow_PlacesIconRelativeToBoundsCenter()
        {
            CreateRig(new Vector3(100f, 0f, 50f), new Vector3(200f, 0f, 150f), new Vector2(100f, 100f));
            var target = rig.CreateTarget("Target", new Vector3(200f, 0f, 150f), Vector3.zero);

            var icon = rig.View.Follow(target);

            AssertVector(new Vector2(50f, 50f), icon.rectTransform.localPosition);
        }

        [Test]
        public void Follow_UsesRectSize_WhenMapStretchesToViewport()
        {
            CreateUniformRig();
            rig.Map.anchorMin = Vector2.zero;
            rig.Map.anchorMax = Vector2.one;
            rig.Map.sizeDelta = Vector2.zero;
            var target = rig.CreateTarget("Target", new Vector3(50f, 0f, 50f), Vector3.zero);

            var icon = rig.View.Follow(target);

            AssertVector(new Vector2(100f, 100f), icon.rectTransform.localPosition);
        }

        [TestCase(MiniMapRotationMode.RotateWithTarget)]
        [TestCase(MiniMapRotationMode.NorthUp)]
        public void FollowCentered_KeepsTargetAtViewportCenter_WithNonUniformMapScale(MiniMapRotationMode mode)
        {
            CreateRig(new Vector3(-50f, 0f, -25f), new Vector3(50f, 0f, 25f), new Vector2(100f, 100f));
            rig.View.RotationMode = mode;
            var player = rig.CreateTarget("Player", new Vector3(20f, 0f, 10f), new Vector3(0f, 90f, 0f));

            rig.View.FollowCentered(player);

            AssertVector(Vector2.zero, rig.WorldPointInViewport(player.position));
        }

        [Test]
        public void FollowCentered_RotateWithTarget_RotatesMapAndKeepsIconUp()
        {
            CreateUniformRig();
            var player = rig.CreateTarget("Player", Vector3.zero, new Vector3(0f, 90f, 0f));

            var icon = rig.View.FollowCentered(player);

            AssertAngle(90f, rig.Map.localEulerAngles.z);
            AssertAngle(0f, icon.rectTransform.localEulerAngles.z);
        }

        [Test]
        public void FollowCentered_NorthUp_KeepsMapStillAndRotatesIcon()
        {
            CreateUniformRig();
            rig.View.RotationMode = MiniMapRotationMode.NorthUp;
            var player = rig.CreateTarget("Player", Vector3.zero, new Vector3(0f, 90f, 0f));

            var icon = rig.View.FollowCentered(player);

            AssertAngle(0f, rig.Map.localEulerAngles.z);
            AssertAngle(-90f, icon.rectTransform.localEulerAngles.z);
        }

        [Test]
        public void FollowCentered_SmallRotationSteps_KeepTargetCentered()
        {
            CreateRig(new Vector3(-50f, 0f, -50f), new Vector3(50f, 0f, 50f), new Vector2(1000f, 1000f));
            var player = rig.CreateTarget("Player", new Vector3(40f, 0f, 40f), Vector3.zero);
            rig.View.FollowCentered(player);

            player.rotation = Quaternion.Euler(0f, 0.1f, 0f);
            rig.View.Refresh();

            AssertVector(Vector2.zero, rig.WorldPointInViewport(player.position));
        }

        [Test]
        public void FollowCentered_IgnoresTargetScale()
        {
            CreateUniformRig();
            var player = rig.CreateTarget("Player", new Vector3(20f, 0f, 10f), Vector3.zero);
            player.localScale = Vector3.one * 2f;

            rig.View.FollowCentered(player);

            AssertVector(new Vector2(-20f, -10f), rig.Map.localPosition);
        }

        [Test]
        public void Zoom_ScalesMapAroundCenteredTarget_AndKeepsIconSize()
        {
            CreateUniformRig();
            rig.View.RotationMode = MiniMapRotationMode.NorthUp;
            rig.View.Zoom = 2f;
            var player = rig.CreateTarget("Player", new Vector3(20f, 0f, 10f), Vector3.zero);
            var other = rig.CreateTarget("Other", new Vector3(-10f, 0f, 0f), Vector3.zero);

            rig.View.FollowCentered(player);
            var otherIcon = rig.View.Follow(other);

            AssertVector(new Vector3(2f, 2f, 1f), rig.Map.localScale);
            AssertVector(new Vector2(-40f, -20f), rig.Map.localPosition);
            AssertVector(Vector2.zero, rig.WorldPointInViewport(player.position));
            AssertVector(new Vector3(0.5f, 0.5f, 1f), otherIcon.rectTransform.localScale);
        }

        [Test]
        public void Zoom_IsClampedAndStepped()
        {
            CreateUniformRig();
            rig.View.SetZoomLimits(1f, 2f);

            rig.View.Zoom = 10f;
            Assert.AreEqual(2f, rig.View.Zoom, Tolerance);
            rig.View.ZoomOut();
            Assert.AreEqual(1.6f, rig.View.Zoom, Tolerance);
            rig.View.Zoom = 0f;
            Assert.AreEqual(1f, rig.View.Zoom, Tolerance);
            rig.View.ZoomIn();
            Assert.AreEqual(1.25f, rig.View.Zoom, Tolerance);
        }

        [Test]
        public void XYPlane_FollowCentered_UsesZRotationAndXYPosition()
        {
            CreateRig(new Vector3(-50f, -50f, 0f), new Vector3(50f, 50f, 0f), new Vector2(100f, 100f), MiniMapPlane.XY);
            var player = rig.CreateTarget("Player", new Vector3(10f, 20f, 5f), new Vector3(0f, 0f, 30f));

            rig.View.FollowCentered(player);

            AssertAngle(-30f, rig.Map.localEulerAngles.z);
            AssertVector(Vector2.zero, rig.WorldPointInViewport(player.position));
        }

        [Test]
        public void Follow_RotateWithTargetFalse_KeepsIconUprightOnRotatingMap()
        {
            CreateUniformRig();
            var player = rig.CreateTarget("Player", Vector3.zero, new Vector3(0f, 90f, 0f));
            var shop = rig.CreateTarget("Shop", new Vector3(10f, 0f, 0f), new Vector3(0f, 45f, 0f));

            rig.View.FollowCentered(player);
            var icon = rig.View.Follow(shop, rotateWithTarget: false);

            AssertAngle(0f, icon.rectTransform.eulerAngles.z);
        }

        [Test]
        public void Follow_NullTarget_ThrowsWithoutCreatingIcon()
        {
            CreateUniformRig();

            Assert.Throws<ArgumentNullException>(() => rig.View.Follow(null));
            Assert.AreEqual(0, rig.Map.childCount);
        }

        [Test]
        public void FollowCentered_ReplacesExistingIconOfSameTarget()
        {
            CreateUniformRig();
            var player = rig.CreateTarget("Player", Vector3.zero, Vector3.zero);

            rig.View.Follow(player);
            rig.View.FollowCentered(player);

            Assert.AreEqual(0, rig.View.FollowedTargetCount);
            Assert.AreEqual(0, rig.Map.childCount);
            Assert.AreSame(player, rig.View.CenteredTarget);
        }

        [Test]
        public void Refresh_RemovesIconsOfDestroyedTargets()
        {
            CreateUniformRig();
            var player = rig.CreateTarget("Player", Vector3.zero, Vector3.zero);
            var other = rig.CreateTarget("Other", new Vector3(5f, 0f, 5f), Vector3.zero);
            rig.View.FollowCentered(player);
            rig.View.Follow(other);

            Object.DestroyImmediate(player.gameObject);
            Object.DestroyImmediate(other.gameObject);
            rig.View.Refresh();

            Assert.IsNull(rig.View.CenteredTarget);
            Assert.AreEqual(0, rig.View.FollowedTargetCount);
            Assert.AreEqual(0, rig.Map.childCount);
            Assert.AreEqual(1, rig.Viewport.childCount);
        }

        [Test]
        public void UnfollowCentered_RestoresAuthoredMapPose()
        {
            CreateUniformRig();
            var player = rig.CreateTarget("Player", new Vector3(20f, 0f, 10f), new Vector3(0f, 90f, 0f));

            rig.View.FollowCentered(player);
            rig.View.UnfollowTarget(player);
            rig.View.Refresh();

            AssertVector(Vector2.zero, rig.Map.localPosition);
            AssertAngle(0f, rig.Map.localEulerAngles.z);
        }

        [Test]
        public void Follow_WithoutSprites_KeepsPrefabSprite()
        {
            CreateUniformRig();
            var texture = rig.Track(new Texture2D(4, 4));
            var sprite = rig.Track(Sprite.Create(texture, new Rect(0f, 0f, 4f, 4f), new Vector2(0.5f, 0.5f)));
            rig.View.uiDotPrefab.sprite = sprite;
            rig.View.defaultSprite = null;

            var icon = rig.View.Follow(rig.CreateTarget("Target", Vector3.zero, Vector3.zero));

            Assert.AreSame(sprite, icon.sprite);
        }

        [Test]
        public void Refresh_WithoutBounds_WarnsOnceAndDoesNotThrow()
        {
            CreateUniformRig();
            rig.View.miniMapBounds = null;
            var target = rig.CreateTarget("Target", Vector3.zero, Vector3.zero);

            LogAssert.Expect(LogType.Warning, new Regex("Cannot update the map"));
            Assert.DoesNotThrow(() => rig.View.Follow(target));
            Assert.DoesNotThrow(() => rig.View.Refresh());
        }

        private void CreateUniformRig()
        {
            CreateRig(new Vector3(-50f, 0f, -50f), new Vector3(50f, 0f, 50f), new Vector2(100f, 100f));
        }

        private void CreateRig(Vector3 bottomLeft, Vector3 topRight, Vector2 mapSize, MiniMapPlane plane = MiniMapPlane.XZ)
        {
            rig = new MiniMapTestRig(bottomLeft, topRight, mapSize, plane);
        }

        private static void AssertVector(Vector3 expected, Vector3 actual)
        {
            Assert.AreEqual(expected.x, actual.x, Tolerance, "x");
            Assert.AreEqual(expected.y, actual.y, Tolerance, "y");
            Assert.AreEqual(expected.z, actual.z, Tolerance, "z");
        }

        private static void AssertAngle(float expected, float actual)
        {
            Assert.AreEqual(0f, Mathf.DeltaAngle(expected, actual), Tolerance, $"expected {expected} but was {actual}");
        }
    }
}
