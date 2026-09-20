using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Arikan
{
    public partial class MiniMapView
    {
        private readonly Dictionary<Transform, MiniMapIconEntry> icons = new Dictionary<Transform, MiniMapIconEntry>();
        private readonly List<Transform> staleTargets = new List<Transform>();
        private MiniMapIconEntry centeredIcon;

        /// <summary>
        /// Target kept at the center of the map, or null.
        /// </summary>
        public Transform CenteredTarget => centeredIcon != null ? centeredIcon.Target : null;

        /// <summary>
        /// Number of followed targets, excluding the centered target.
        /// </summary>
        public int FollowedTargetCount => icons.Count;

        public bool IsFollowing(Transform target)
        {
            if (target == null)
            {
                return false;
            }

            return icons.ContainsKey(target) || (centeredIcon != null && centeredIcon.Target == target);
        }

        /// <summary>
        /// Keeps the target at the center of the map and returns its icon.
        /// Replaces the previous centered target and any existing icon of this target.
        /// </summary>
#if ODIN_INSPECTOR
        [Button]
#endif
        public Image FollowCentered(Transform target, Sprite icon = null)
        {
            ValidateFollow(target, centeredDotCanvas, nameof(centeredDotCanvas));
            UnfollowCentered();
            UnfollowTarget(target);

            var image = CreateIcon(target, icon, centeredDotCanvas);
            centeredIcon = new MiniMapIconEntry(target, image, rotateWithTarget: true);
            Refresh();
            return image;
        }

        /// <summary>
        /// Shows the target on the map and returns its icon. Replaces any existing icon of this target.
        /// </summary>
        /// <param name="target">Transform to follow.</param>
        /// <param name="icon">Sprite of the icon. Falls back to the default sprite, then to the prefab sprite.</param>
        /// <param name="rotateWithTarget">True to turn the icon with the target, false to keep it upright on screen.</param>
#if ODIN_INSPECTOR
        [Button]
#endif
        public Image Follow(Transform target, Sprite icon = null, bool rotateWithTarget = true)
        {
            ValidateFollow(target, otherDotCanvas, nameof(otherDotCanvas));
            UnfollowTarget(target);

            var image = CreateIcon(target, icon, otherDotCanvas);
            icons.Add(target, new MiniMapIconEntry(target, image, rotateWithTarget));
            Refresh();
            return image;
        }

        /// <summary>
        /// Removes the icon of the target. Destroyed targets are accepted.
        /// </summary>
#if ODIN_INSPECTOR
        [Button]
#endif
        public void UnfollowTarget(Transform target)
        {
            if (ReferenceEquals(target, null))
            {
                return;
            }

            if (centeredIcon != null && centeredIcon.Target == target)
            {
                UnfollowCentered();
            }

            if (icons.TryGetValue(target, out var entry))
            {
                icons.Remove(target);
                DestroyIcon(entry);
            }
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        public void ClearTargets()
        {
            UnfollowCentered();
            foreach (var entry in icons.Values)
            {
                DestroyIcon(entry);
            }
            icons.Clear();
        }

        private void UnfollowCentered()
        {
            if (centeredIcon == null)
            {
                return;
            }

            DestroyIcon(centeredIcon);
            centeredIcon = null;
            ResetMapPose();
        }

        private void RemoveDestroyedTargets()
        {
            if (centeredIcon != null && !centeredIcon.IsAlive)
            {
                UnfollowCentered();
            }

            staleTargets.Clear();
            foreach (var pair in icons)
            {
                if (!pair.Value.IsAlive)
                {
                    staleTargets.Add(pair.Key);
                }
            }

            foreach (var target in staleTargets)
            {
                UnfollowTarget(target);
            }
            staleTargets.Clear();
        }

        private void ValidateFollow(Transform target, RectTransform parent, string parentName)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            if (parent == null)
            {
                throw new InvalidOperationException($"[MiniMapView] {parentName} is not assigned.");
            }
            if (uiDotPrefab == null)
            {
                throw new InvalidOperationException($"[MiniMapView] {nameof(uiDotPrefab)} is not assigned.");
            }
        }

        private Image CreateIcon(Transform target, Sprite icon, RectTransform parent)
        {
            var image = Instantiate(uiDotPrefab, parent, false);
            image.name = target.name + " Icon";

            var sprite = icon != null ? icon : defaultSprite;
            if (sprite != null)
            {
                image.sprite = sprite;
            }
            return image;
        }

        private static void DestroyIcon(MiniMapIconEntry entry)
        {
            if (entry.Rect != null)
            {
                MiniMapObjectUtility.Destroy(entry.Rect.gameObject);
            }
        }
    }
}
