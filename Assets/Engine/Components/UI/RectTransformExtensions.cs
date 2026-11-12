using UnityEngine;

namespace StateEngine.Components.UI
{
    public static class RectTransformExtensions
    {
        private static readonly Vector3[] s_Corners = new Vector3[4];

        /// <summary>
        /// Resizes the rect so that it encloses all of its active children.
        /// </summary>
        public static Vector2 FitToChildren(this RectTransform rectTransform)
        {
            Vector2 size = rectTransform.GetChildrenBounds().size;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            return size;
        }

        public static float FitWidthToChildren(this RectTransform rectTransform)
        {
            float width = rectTransform.GetChildrenBounds().width;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            return width;
        }

        public static float FitHeightToChildren(this RectTransform rectTransform)
        {
            float height = rectTransform.GetChildrenBounds().height;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            return height;
        }

        /// <summary>
        /// Bounds of all active child rects in the local space of <paramref name="rectTransform"/>.
        /// Returns an empty rect when there are no active children.
        /// </summary>
        public static Rect GetChildrenBounds(this RectTransform rectTransform)
        {
            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);
            bool hasChildren = false;

            for (int i = 0; i < rectTransform.childCount; i++)
            {
                if (!(rectTransform.GetChild(i) is RectTransform child) || !child.gameObject.activeSelf)
                    continue;

                child.GetWorldCorners(s_Corners);
                for (int c = 0; c < s_Corners.Length; c++)
                {
                    Vector2 local = rectTransform.InverseTransformPoint(s_Corners[c]);
                    min = Vector2.Min(min, local);
                    max = Vector2.Max(max, local);
                }

                hasChildren = true;
            }

            return hasChildren ? Rect.MinMaxRect(min.x, min.y, max.x, max.y) : Rect.zero;
        }
    }
}
