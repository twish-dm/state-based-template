using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class LayoutCalculator
{
    public static Vector2 ApplyCalculatedSize(this RectTransform rectTransform)
    {
        float requiredWidth = CalculateRequiredWidth(rectTransform);
        float requiredHeight = CalculateRequiredHeight(rectTransform);

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, requiredWidth);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, requiredHeight);
        return new Vector2(requiredWidth, requiredHeight);
    }

    public static float ApplyCalculatedHeight(this RectTransform rectTransform)
    {
        float requiredHeight = CalculateRequiredHeight(rectTransform);

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, requiredHeight);
        return requiredHeight;
    }
    public static float ApplyCalculatedWidth(this RectTransform rectTransform)
    {
        float requiredWidth = CalculateRequiredWidth(rectTransform);

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, requiredWidth);
        return requiredWidth;
    }
    private static float CalculateRequiredWidth(RectTransform rectTransform)
    {
        float requiredWidthFixed = 0f;
        float requiredWidthStretched = 0f;
        float minX = float.MaxValue;
        float maxX = float.MinValue;

        for (int i = 0; i < rectTransform.childCount; i++)
        {
            RectTransform child = rectTransform.GetChild(i) as RectTransform;
            if (child == null) continue;

            bool isStretched = !Mathf.Approximately(child.anchorMin.x, child.anchorMax.x);

            if (isStretched)
            {
                float left = child.offsetMin.x;
                float right = -child.offsetMax.x;
                requiredWidthStretched = Mathf.Max(requiredWidthStretched, left + right);
            }
            else
            {
                Vector3[] corners = new Vector3[4];
                child.GetWorldCorners(corners);
                foreach (Vector3 corner in corners)
                {
                    Vector3 localCorner = rectTransform.InverseTransformPoint(corner);
                    minX = Mathf.Min(minX, localCorner.x);
                    maxX = Mathf.Max(maxX, localCorner.x);
                }
            }
        }

        requiredWidthFixed = (minX < maxX) ? (maxX - minX) : 0f;
        return Mathf.Max(requiredWidthFixed, requiredWidthStretched);
    }

    private static float CalculateRequiredHeight(RectTransform rectTransform)
    {
        float requiredHeightFixed = 0f;
        float requiredHeightStretched = 0f;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        for (int i = 0; i < rectTransform.childCount; i++)
        {
            RectTransform child = rectTransform.GetChild(i) as RectTransform;
            if (child == null) continue;

            bool isStretched = !Mathf.Approximately(child.anchorMin.y, child.anchorMax.y);

            if (isStretched)
            {
                float bottom = child.offsetMin.y;
                float top = -child.offsetMax.y;
                requiredHeightStretched = Mathf.Max(requiredHeightStretched, bottom + top);
            }
            else
            {
                Vector3[] corners = new Vector3[4];
                child.GetWorldCorners(corners);
                foreach (Vector3 corner in corners)
                {
                    Vector3 localCorner = rectTransform.InverseTransformPoint(corner);
                    minY = Mathf.Min(minY, localCorner.y);
                    maxY = Mathf.Max(maxY, localCorner.y);
                }
            }
        }

        requiredHeightFixed = (minY < maxY) ? (maxY - minY) : 0f;
        return Mathf.Max(requiredHeightFixed, requiredHeightStretched);
    }
}