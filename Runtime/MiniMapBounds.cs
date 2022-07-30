using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Arikan
{
    public class MiniMapBounds : MonoBehaviour
    {
        public Transform topRight;
        public Transform bottomLeft;

        public Bounds GetWorldRect()
        {
            var size = topRight.position - bottomLeft.position;
            var center = bottomLeft.position + size / 2;
            return new Bounds(center, size);
        }

        private void DetectChilds()
        {

            if (topRight == null)
            {
                if (transform.childCount > 0)
                {
                    topRight = transform.GetChild(0);
                }
                else
                {
                    var obj = new GameObject("TopRight").transform;
                    obj.SetParent(transform);
                    transform.localPosition = new Vector3(-1, 0, -1);
                    obj.SetSiblingIndex(0);
                    topRight = obj;
                }
            }
            if (bottomLeft == null)
            {
                if (transform.childCount > 1)
                {
                    bottomLeft = transform.GetChild(1);
                }
                else
                {
                    var obj = new GameObject("BottomLeft").transform;
                    obj.SetParent(transform);
                    transform.localPosition = new Vector3(1, 0, 1);
                    obj.SetSiblingIndex(1);
                    bottomLeft = obj;
                }
            }
        }
        private void OnValidate()
        {
            DetectChilds();
        }
        private void OnEnable()
        {
#if UNITY_EDITOR
            if (transform.lossyScale != Vector3.one)
            {
                Debug.LogError("[MiniMapBounds] transform.lossyScale != Vector3.one, this causes wrong positions over minimap", transform);
            }
            if (transform.rotation != Quaternion.identity)
            {
                Debug.LogError("[MiniMapBounds] transform.rotation != Quaternion.identity, this causes wrong positions over minimap", transform);
            }
#endif
            DetectChilds();
        }

        private void OnDrawGizmosSelected()
        {
            if (topRight != null && bottomLeft != null)
            {
                var worldBounds = GetWorldRect();
                Gizmos.DrawWireCube(worldBounds.center, worldBounds.size);
            }
        }
    }
}