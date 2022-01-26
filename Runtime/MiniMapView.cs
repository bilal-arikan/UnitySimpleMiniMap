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
        [Header("MiniMap World Center")]
        public Transform origin;

        [Header("RectTransform Roots")]
        public RectTransform centeredDotCanvas;
        public RectTransform otherDotCanvas;
        [Header("Defult Sprite")]
        public Sprite defaultSprite;
        [Header("Default Dot Prefab")]
        public Image uiDotPrefab;

        private Dictionary<Transform, RectTransform> redDotMap = new Dictionary<Transform, RectTransform>();
        private KeyValuePair<Transform, RectTransform> mainMap = new KeyValuePair<Transform, RectTransform>();
        private Bounds worldBounds = new Bounds();

        private void OnEnable()
        {
            if (origin == null)
            {
                origin = new GameObject("MiniMapOrigin_Generated").transform;
                worldBounds = new Bounds(origin.position, Vector3.one);
            }
            else if (origin.TryGetComponent<Renderer>(out var renderer))
            {
                worldBounds = renderer.bounds;
            }
            else if (origin.TryGetComponent<Collider>(out var collider))
            {
                worldBounds = collider.bounds;
            }
            else if (origin.TryGetComponent<Collider2D>(out var collider2D))
            {
                worldBounds = collider2D.bounds;
            }
            else
            {
                Debug.LogWarning("No Renderer or Collider found on the origin, world bounds and sprite bounds matching may be wrong!");
                worldBounds = new Bounds(origin.position, Vector3.one);
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
                Debug.LogWarning("[MiniMap] target.lossyScale != 1, this causes wrong positions over minimap", target);
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
            var sizeDif = new Vector3(
                otherDotCanvas.sizeDelta.x / worldBounds.size.x,
                1,
                otherDotCanvas.sizeDelta.y / worldBounds.size.z
            );

            var m = this.origin.worldToLocalMatrix * worldObj.localToWorldMatrix;

            dot.localPosition = Vector3.Scale(sizeDif, m.GetPosition()).XZ();
            dot.localEulerAngles = new Vector3(0, 0, -m.GetRotation().eulerAngles.y);
        }

        public void TranslateReverse(Transform worldObj, RectTransform dot)
        {
            var sizeDif = new Vector3(
                otherDotCanvas.sizeDelta.x / worldBounds.size.x,
                1,
                otherDotCanvas.sizeDelta.y / worldBounds.size.z
            );

            var m = worldObj.worldToLocalMatrix * this.origin.localToWorldMatrix;

            otherDotCanvas.localPosition = Vector3.Scale(sizeDif, m.GetPosition()).XZ();
            otherDotCanvas.localEulerAngles = new Vector3(0, 0, -m.GetRotation().eulerAngles.y);
        }

        private void OnDrawGizmosSelected()
        {
            if (origin != null)
            {
                Gizmos.DrawWireCube(origin.position, worldBounds.size);
            }
        }
    }

}
