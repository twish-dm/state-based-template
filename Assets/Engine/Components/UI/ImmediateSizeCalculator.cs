using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
[ExecuteAlways]
public class ImmediateSizeCalculator : MonoBehaviour
{
    public int paddingLeft;
    public int paddingRight;
    public int paddingTop;
    public int paddingBottom;

    private RectTransform rootRect;
    private List<ComponentState> layoutComponents = new List<ComponentState>();

    private struct ComponentState
    {
        public Behaviour component;
        public bool enabled;
    }

    void OnEnable() => CalculateSize();

    [ContextMenu("Calculate Size")]
    public void CalculateSize()
    {
        if (!TryGetComponent(out rootRect)) return;

        // Сохраняем и отключаем все автоматические компоненты
        DisableAutoLayoutComponents();

        // Принудительное обновление всей иерархии
        ForceImmediateLayoutRebuild(rootRect);

        // Рассчитываем и устанавливаем размер
        Vector2 contentSize = CalculateContentBounds();
        ApplyNewSize(contentSize);

        // Восстанавливаем компоненты
        RestoreComponents();
    }

    private void DisableAutoLayoutComponents()
    {
        layoutComponents.Clear();

        // Обрабатываем все компоненты в иерархии
        foreach (var component in GetComponentsInChildren<Behaviour>(true))
        {
            if (component is LayoutGroup || component is ContentSizeFitter)
            {
                layoutComponents.Add(new ComponentState
                {
                    component = component,
                    enabled = component.enabled
                });
                component.enabled = false;
            }
        }
    }

    private void ForceImmediateLayoutRebuild(RectTransform parent)
    {
        // Рекурсивное обновление снизу вверх
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            RectTransform child = parent.GetChild(i) as RectTransform;
            if (child) ForceImmediateLayoutRebuild(child);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(parent);
        Canvas.ForceUpdateCanvases();
    }

    private Vector2 CalculateContentBounds()
    {
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);

        foreach (RectTransform child in GetActiveChildren(rootRect))
        {
            UpdateBounds(child, ref min, ref max);
        }

        return new Vector2(
            Mathf.Abs(max.x - min.x),
            Mathf.Abs(max.y - min.y)
        );
    }

    private void ApplyNewSize(Vector2 contentSize)
    {
        Vector2 finalSize = new Vector2(
            contentSize.x + paddingLeft + paddingRight,
            contentSize.y + paddingTop + paddingBottom
        );

        // Устанавливаем размер через sizeDelta с учетом текущих анкоров
        rootRect.sizeDelta = new Vector2(
            finalSize.x - GetHorizontalAnchorSize(),
            finalSize.y - GetVerticalAnchorSize()
        );
    }

    private float GetHorizontalAnchorSize()
    {
        Rect parentRect = rootRect.parent.GetComponent<RectTransform>().rect;
        return parentRect.width * (rootRect.anchorMax.x - rootRect.anchorMin.x);
    }

    private float GetVerticalAnchorSize()
    {
        Rect parentRect = rootRect.parent.GetComponent<RectTransform>().rect;
        return parentRect.height * (rootRect.anchorMax.y - rootRect.anchorMin.y);
    }

    private void RestoreComponents()
    {
        foreach (var state in layoutComponents)
        {
            if (state.component)
                state.component.enabled = state.enabled;
        }
        layoutComponents.Clear();
    }

    private List<RectTransform> GetActiveChildren(RectTransform parent)
    {
        List<RectTransform> children = new List<RectTransform>();
        foreach (RectTransform child in parent)
        {
            if (child.gameObject.activeInHierarchy)
                children.Add(child);
        }
        return children;
    }

    private void UpdateBounds(RectTransform child, ref Vector2 min, ref Vector2 max)
    {
        Vector3[] corners = new Vector3[4];
        child.GetWorldCorners(corners);

        foreach (Vector3 corner in corners)
        {
            Vector2 localCorner = rootRect.InverseTransformPoint(corner);

            // Коррекция для pivot и масштаба
            Vector2 pivotOffset = new Vector2(
                child.rect.width * (child.pivot.x - 0.5f) * child.localScale.x,
                child.rect.height * (child.pivot.y - 0.5f) * child.localScale.y
            );

            localCorner += pivotOffset;

            min.x = Mathf.Min(min.x, localCorner.x);
            min.y = Mathf.Min(min.y, localCorner.y);
            max.x = Mathf.Max(max.x, localCorner.x);
            max.y = Mathf.Max(max.y, localCorner.y);
        }
    }
}