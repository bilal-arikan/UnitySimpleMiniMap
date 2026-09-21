using Arikan;
using UnityEngine;

namespace Arikan
{
    public class MiniMapDemo : MonoBehaviour
    {
        public MeshRenderer obj1Centered;
        public MeshRenderer obj2;
        public MeshRenderer obj3;

        public Sprite obj3Sprite;

        [Header("Motion")]
        [Tooltip("Radius of the loop walked by the centered object.")]
        public float loopRadius = 8f;

        [Tooltip("Speed along the loop in degrees per second.")]
        public float loopSpeed = 25f;

        private MiniMapView minimap;
        private Vector3 loopCenter;
        private float loopAngle;

        private void Start()
        {
#if UNITY_2023_1_OR_NEWER
            minimap = FindFirstObjectByType<MiniMapView>();
#else
            minimap = FindObjectOfType<MiniMapView>();
#endif
            if (minimap == null)
            {
                Debug.LogError("[MiniMapDemo] No MiniMapView found in the scene.", this);
                enabled = false;
                return;
            }

            // Red object stays at the center of the mini map.
            minimap.FollowCentered(obj1Centered.transform).color = GetColor(obj1Centered);

            // Green object turns with its transform.
            minimap.Follow(obj2.transform).color = GetColor(obj2);

            // Blue object uses a custom sprite that stays upright.
            minimap.Follow(obj3.transform, obj3Sprite, rotateWithTarget: false).color = GetColor(obj3);

            loopCenter = obj1Centered.transform.position - new Vector3(loopRadius, 0f, 0f);
        }

        private void Update()
        {
            loopAngle += loopSpeed * Time.deltaTime;
            var radians = loopAngle * Mathf.Deg2Rad;
            var offset = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians)) * loopRadius;
            var direction = new Vector3(-Mathf.Sin(radians), 0f, Mathf.Cos(radians));

            obj1Centered.transform.SetPositionAndRotation(loopCenter + offset, Quaternion.LookRotation(direction, Vector3.up));
        }

        private void OnGUI()
        {
            if (minimap == null)
            {
                return;
            }

            GUILayout.BeginArea(new Rect(Screen.width - 230f, 10f, 220f, 120f), GUI.skin.box);
            GUILayout.Label("Rotation: " + minimap.RotationMode);
            if (GUILayout.Button("Toggle Rotation Mode"))
            {
                minimap.RotationMode = minimap.RotationMode == MiniMapRotationMode.NorthUp
                    ? MiniMapRotationMode.RotateWithTarget
                    : MiniMapRotationMode.NorthUp;
            }

            GUILayout.Label($"Zoom: {minimap.Zoom:0.00}x");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Zoom -"))
            {
                minimap.ZoomOut();
            }
            if (GUILayout.Button("Zoom +"))
            {
                minimap.ZoomIn();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private static Color GetColor(Renderer target)
        {
            var material = target.sharedMaterial;
            if (material == null)
            {
                return Color.white;
            }
            if (material.HasProperty("_BaseColor"))
            {
                return material.GetColor("_BaseColor");
            }
            return material.HasProperty("_Color") ? material.GetColor("_Color") : Color.white;
        }
    }
}