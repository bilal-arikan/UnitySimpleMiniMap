using UnityEngine;

namespace Arikan
{
    public partial class MiniMapView
    {
        private bool mapPoseCaptured;
        private Vector3 initialMapPosition;
        private Quaternion initialMapRotation = Quaternion.identity;
        private Vector3 initialMapScale = Vector3.one;
        private bool layoutWarningLogged;

        /// <summary>
        /// Updates the map and every icon immediately. Called automatically in LateUpdate.
        /// </summary>
        public void Refresh()
        {
            RemoveDestroyedTargets();
            if (!TryGetLayout(out var worldRect, out var plane))
            {
                return;
            }

            CaptureMapPose();
            var mapRect = otherDotCanvas.rect;
            var mapScale = new Vector3(initialMapScale.x * zoom, initialMapScale.y * zoom, initialMapScale.z);
            var mapAngle = centeredIcon != null
                ? LayoutCenteredTarget(worldRect, plane, mapRect, mapScale)
                : LayoutFreeMap(mapScale);

            LayoutIcons(worldRect, plane, mapRect, mapAngle);
        }

        /// <summary>
        /// Moves the map so the centered target sits at the viewport center. Returns the map angle.
        /// </summary>
        private float LayoutCenteredTarget(Rect worldRect, MiniMapPlane plane, Rect mapRect, Vector3 mapScale)
        {
            var target = centeredIcon.Target;
            var targetAngle = MiniMapMath.GetMapAngle(target, plane);
            var rotateMap = rotationMode == MiniMapRotationMode.RotateWithTarget;
            var mapAngle = rotateMap ? -targetAngle : 0f;
            var mapRotation = Quaternion.Euler(0f, 0f, mapAngle);

            var targetOnMap = mapRect.center + MiniMapMath.WorldToMap(target.position, worldRect, mapRect.size, plane);
            var offset = mapRotation * Vector3.Scale(mapScale, targetOnMap);
            var mapPosition = GetParentCenter(otherDotCanvas) - new Vector2(offset.x, offset.y);
            SetPose(otherDotCanvas, mapPosition, mapRotation, mapScale);

            var icon = centeredIcon.Rect;
            var iconRotation = Quaternion.Euler(0f, 0f, rotateMap ? 0f : targetAngle);
            SetPose(icon, GetParentCenter(icon), iconRotation, icon.localScale);
            return mapAngle;
        }

        /// <summary>
        /// Keeps the authored map pose when nothing is centered. Returns the map angle.
        /// </summary>
        private float LayoutFreeMap(Vector3 mapScale)
        {
            SetPose(otherDotCanvas, initialMapPosition, initialMapRotation, mapScale);
            return initialMapRotation.eulerAngles.z;
        }

        private void LayoutIcons(Rect worldRect, MiniMapPlane plane, Rect mapRect, float mapAngle)
        {
            foreach (var entry in icons.Values)
            {
                var position = mapRect.center + MiniMapMath.WorldToMap(entry.Target.position, worldRect, mapRect.size, plane);
                var angle = entry.RotateWithTarget ? MiniMapMath.GetMapAngle(entry.Target, plane) : -mapAngle;
                var scale = new Vector3(entry.BaseScale.x / zoom, entry.BaseScale.y / zoom, entry.BaseScale.z);
                SetPose(entry.Rect, position, Quaternion.Euler(0f, 0f, angle), scale);
            }
        }

        private bool TryGetLayout(out Rect worldRect, out MiniMapPlane plane)
        {
            worldRect = default;
            plane = MiniMapPlane.XZ;

            string problem = null;
            if (otherDotCanvas == null)
            {
                problem = nameof(otherDotCanvas) + " is not assigned";
            }
            else if (miniMapBounds == null)
            {
                problem = nameof(miniMapBounds) + " is not assigned";
            }
            else if (!miniMapBounds.IsValid)
            {
                problem = "the MiniMapBounds corners are missing or span an empty area on the selected plane";
            }
            else if (otherDotCanvas.rect.width <= 0f || otherDotCanvas.rect.height <= 0f)
            {
                problem = nameof(otherDotCanvas) + " has an empty rect";
            }

            if (problem != null)
            {
                if (!layoutWarningLogged && (centeredIcon != null || icons.Count > 0))
                {
                    Debug.LogWarning("[MiniMapView] Cannot update the map: " + problem + ".", this);
                    layoutWarningLogged = true;
                }
                return false;
            }

            layoutWarningLogged = false;
            worldRect = miniMapBounds.GetPlaneRect();
            plane = miniMapBounds.plane;
            return true;
        }

        private void CaptureMapPose()
        {
            if (mapPoseCaptured)
            {
                return;
            }

            initialMapPosition = otherDotCanvas.localPosition;
            initialMapRotation = otherDotCanvas.localRotation;
            initialMapScale = otherDotCanvas.localScale;
            mapPoseCaptured = true;
        }

        private void ResetMapPose()
        {
            if (!mapPoseCaptured || otherDotCanvas == null)
            {
                return;
            }

            otherDotCanvas.localPosition = initialMapPosition;
            otherDotCanvas.localRotation = initialMapRotation;
            otherDotCanvas.localScale = initialMapScale;
        }

        private static Vector2 GetParentCenter(Transform child)
        {
            return child.parent is RectTransform parent ? parent.rect.center : Vector2.zero;
        }

        // Exact comparisons: Unity's == operators ignore small changes (about 0.16 degrees for rotations),
        // which would leave the map slightly behind a slowly turning target.
        private static void SetPose(Transform target, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if (!target.localPosition.Equals(position))
            {
                target.localPosition = position;
            }
            if (!target.localRotation.Equals(rotation))
            {
                target.localRotation = rotation;
            }
            if (!target.localScale.Equals(scale))
            {
                target.localScale = scale;
            }
        }
    }
}
