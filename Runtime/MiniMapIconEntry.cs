using UnityEngine;
using UnityEngine.UI;

namespace Arikan
{
    /// <summary>
    /// Runtime state of one followed target.
    /// </summary>
    internal sealed class MiniMapIconEntry
    {
        public readonly Transform Target;
        public readonly RectTransform Rect;
        public readonly bool RotateWithTarget;
        public readonly Vector3 BaseScale;

        public MiniMapIconEntry(Transform target, Image image, bool rotateWithTarget)
        {
            Target = target;
            Rect = image.rectTransform;
            RotateWithTarget = rotateWithTarget;
            BaseScale = Rect.localScale;
        }

        /// <summary>
        /// False once the target or its icon has been destroyed.
        /// </summary>
        public bool IsAlive => Target != null && Rect != null;
    }
}
