using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Arikan
{
    public class MiniMapView : MonoBehaviour
    {
        [Header("RectTransform Roots")]
        public RectTransform centeredDotCanvas;
        public RectTransform otherDotCanvas;
        [Header("Defult Sprite")]
        public Sprite defaultSprite;
        [Header("Default Dot Prefab")]
        public Image uiDotPrefab;
#if ODIN_INSPECTOR
        [Required]
#endif
        [Header("Bounds Object")]
        public MiniMapBounds miniMapBounds;

        private Dictionary<Transform, RectTransform> redDotMap = new Dictionary<Transform, RectTransform>();
        private KeyValuePair<Transform, RectTransform> mainMap = new KeyValuePair<Transform, RectTransform>();

        private void OnEnable()
        {
            if (miniMapBounds == null)
            {
                miniMapBounds = FindObjectOfType<MiniMapBounds>();
            }
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        /// <summary>
        /// Follow target over the minimap, returns Generated MiniMap Image object
        /// </summary>
        public Image FollowCentered(Transform target, Sprite icon = null)
        {
            if (centeredDotCanvas == null)
            {
                throw new NullReferenceException("[MiniMapView] centeredDotCanvas is null");
            }
            if (uiDotPrefab == null)
            {
                throw new NullReferenceException("[MiniMapView] uiDotPrefab is null");
            }
            if (target.lossyScale.x != 1)
            {
                Debug.LogWarning("[MiniMapView] target.lossyScale != 1, this causes wrong positions over minimap", target);
            }
            if (mainMap.Key != null)
            {
                UnfollowTarget(mainMap.Key);
            }

            var uiDot = Instantiate(uiDotPrefab, centeredDotCanvas);
            uiDot.sprite = icon ?? defaultSprite;
            mainMap = new KeyValuePair<Transform, RectTransform>(target, uiDot.transform as RectTransform);
            return uiDot;
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        /// <summary>
        /// Follow target over the minimap, returns Generated MiniMap Image object
        /// </summary>
        public Image Follow(Transform target, Sprite icon = null)
        {
            if (otherDotCanvas == null)
            {
                throw new NullReferenceException("[MiniMapView] otherDotCanvas is null");
            }
            if (uiDotPrefab == null)
            {
                throw new NullReferenceException("[MiniMapView] uiDotPrefab is null");
            }
            UnfollowTarget(target);

            var uiDot = Instantiate(uiDotPrefab, otherDotCanvas);
            uiDot.sprite = icon ?? defaultSprite;
            redDotMap.Add(target, uiDot.transform as RectTransform);
            return uiDot;
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        public void UnfollowTarget(Transform target)
        {
            if (mainMap.Key == target)
            {
                if (mainMap.Value != null)
                    Destroy(mainMap.Value.gameObject);
                mainMap = new KeyValuePair<Transform, RectTransform>();
            }
            else if (redDotMap.TryGetValue(target, out var redDot))
            {
                if (redDot != null)
                    Destroy(redDot.gameObject);
                redDotMap.Remove(target);
            }
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        public void ClearTargets()
        {
            if (mainMap.Key != null)
            {
                UnfollowTarget(mainMap.Key);
            }
            foreach (var redDot in redDotMap.ToList())
            {
                UnfollowTarget(redDot.Key);
            }
        }

        private void Update()
        {
            if (mainMap.Key != null)
            {
                var target = mainMap.Key;
                var redDot = mainMap.Value;

                TranslateReverse(target, redDot);
            }

            foreach (var pair in redDotMap)
            {
                var target = pair.Key;
                var redDot = pair.Value;

                if (target != null)
                {
                    Translate(target, redDot);
                }
            }
        }


        public void Translate(Transform worldObj, RectTransform dot)
        {
            var worldBounds = miniMapBounds.GetWorldRect();
            var sizeDif = new Vector3(
                otherDotCanvas.sizeDelta.x / worldBounds.size.x,
                1,
                otherDotCanvas.sizeDelta.y / worldBounds.size.z
            );

            var originWorldToLocal = Matrix4x4.TRS(worldBounds.center, Quaternion.identity, Vector3.one);
            var m = originWorldToLocal * worldObj.localToWorldMatrix;

            dot.localPosition = Vector3.Scale(sizeDif, m.GetPosition()).XZ();
            dot.localEulerAngles = new Vector3(0, 0, -m.GetRotation().eulerAngles.y);
        }

        public void TranslateReverse(Transform worldObj, RectTransform dot)
        {
            var worldBounds = miniMapBounds.GetWorldRect();
            var sizeDif = new Vector3(
                otherDotCanvas.sizeDelta.x / worldBounds.size.x,
                1,
                otherDotCanvas.sizeDelta.y / worldBounds.size.z
            );

            var originLocalToWorld = Matrix4x4.TRS(-worldBounds.center, Quaternion.identity, Vector3.one);
            var m = worldObj.worldToLocalMatrix * originLocalToWorld;

            otherDotCanvas.localPosition = Vector3.Scale(sizeDif, m.GetPosition()).XZ();
            otherDotCanvas.localEulerAngles = new Vector3(0, 0, -m.GetRotation().eulerAngles.y);
        }

        private void OnDrawGizmosSelected()
        {
            if (miniMapBounds != null)
            {
                var worldBounds = miniMapBounds.GetWorldRect();
                Gizmos.DrawWireCube(worldBounds.center, worldBounds.size);
            }
        }
    }

}
