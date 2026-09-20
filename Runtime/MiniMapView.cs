using UnityEngine;
using UnityEngine.UI;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Arikan
{
    /// <summary>
    /// Shows followed targets on a map image without extra cameras or render textures.
    /// </summary>
    public partial class MiniMapView : MonoBehaviour
    {
        private const float MinimumZoom = 0.01f;
        private const float MinimumZoomStep = 1.01f;

        [Header("RectTransform Roots")]
        [Tooltip("Viewport of the map, usually the mask. The centered target is shown at its center.")]
        public RectTransform centeredDotCanvas;

        [Tooltip("Map image rect. It must show exactly the MiniMapBounds area and be a direct child of the viewport.")]
        public RectTransform otherDotCanvas;

        [Header("Default Sprite")]
        public Sprite defaultSprite;

        [Header("Default Dot Prefab")]
        public Image uiDotPrefab;

#if ODIN_INSPECTOR
        [Required]
#endif
        [Header("Bounds Object")]
        public MiniMapBounds miniMapBounds;

        [Header("Rotation")]
        [Tooltip("NorthUp keeps world up at the top. RotateWithTarget turns the map with the centered target.")]
        [SerializeField]
        private MiniMapRotationMode rotationMode = MiniMapRotationMode.RotateWithTarget;

        [Header("Zoom")]
        [SerializeField, Min(MinimumZoom)]
        private float zoom = 1f;

        [SerializeField, Min(MinimumZoom)]
        private float minZoom = 0.5f;

        [SerializeField, Min(MinimumZoom)]
        private float maxZoom = 4f;

        [Tooltip("Multiplier applied by ZoomIn and ZoomOut.")]
        [SerializeField, Min(MinimumZoomStep)]
        private float zoomStep = 1.25f;

        /// <summary>
        /// How the map rotates while a centered target is followed.
        /// </summary>
        public MiniMapRotationMode RotationMode
        {
            get => rotationMode;
            set => rotationMode = value;
        }

        /// <summary>
        /// Current zoom factor, clamped between <see cref="MinZoom"/> and <see cref="MaxZoom"/>.
        /// </summary>
        public float Zoom
        {
            get => zoom;
            set => zoom = Mathf.Clamp(value, minZoom, maxZoom);
        }

        public float MinZoom => minZoom;

        public float MaxZoom => maxZoom;

        /// <summary>
        /// Sets the zoom limits and clamps the current zoom into them.
        /// </summary>
        public void SetZoomLimits(float min, float max)
        {
            minZoom = Mathf.Max(MinimumZoom, Mathf.Min(min, max));
            maxZoom = Mathf.Max(minZoom, Mathf.Max(min, max));
            Zoom = zoom;
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        public void ZoomIn()
        {
            Zoom = zoom * zoomStep;
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        public void ZoomOut()
        {
            Zoom = zoom / zoomStep;
        }

        private void OnEnable()
        {
            if (miniMapBounds == null)
            {
                miniMapBounds = MiniMapObjectUtility.FindFirst<MiniMapBounds>();
            }
        }

        private void LateUpdate()
        {
            Refresh();
        }

        private void OnValidate()
        {
            minZoom = Mathf.Max(MinimumZoom, minZoom);
            maxZoom = Mathf.Max(minZoom, maxZoom);
            zoomStep = Mathf.Max(MinimumZoomStep, zoomStep);
            zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        }
    }
}
