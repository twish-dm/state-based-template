using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public static class LayoutSizeCalculator
{
    public static IEnumerator CalculateTotalSize(RectTransform root, System.Action<Vector2> callback)
    {
        Vector2 calculatedSize = Vector2.zero;

        // Рекурсивный расчет
        yield return CalculateNestedSize(root, (size) => {
            calculatedSize = size;
        });

        callback?.Invoke(calculatedSize);
    }

    private static IEnumerator CalculateNestedSize(RectTransform target, System.Action<Vector2> onComplete)
    {
        Vector2 size = Vector2.zero;
        List<IEnumerator> calculations = new List<IEnumerator>();

        // Обработка текущего элемента
        if (TryGetContentSize(target, out Vector2 contentSize))
        {
            size = contentSize;
        }
        else
        {
            // Расчет для Layout Groups
            var layoutGroup = target.GetComponent<LayoutGroup>();
            if (layoutGroup != null)
            {
                yield return HandleLayoutGroup(layoutGroup, (s) => size = s);
            }
            else
            {
                // Прямой расчет для обычных элементов
                size = CalculateSimpleSize(target);
            }
        }

        // Рекурсивный расчет для дочерних элементов
        foreach (RectTransform child in target)
        {
            calculations.Add(CalculateChildSize(child, (childSize) =>
            {
                // Обновление размера на основе дочерних элементов
                size = CombineSizes(size, childSize, target);
            }));
        }

        // Ожидаем завершения всех расчетов
        foreach (var calc in calculations)
        {
            yield return calc;
        }

        onComplete?.Invoke(size);
    }
    private static IEnumerator CalculateChildSize(RectTransform child, System.Action<Vector2> onChildSizeCalculated)
    {
        // Учет активности объекта
        if (!child.gameObject.activeInHierarchy)
        {
            onChildSizeCalculated?.Invoke(Vector2.zero);
            yield break;
        }

        // Рекурсивный расчет для вложенных элементов
        yield return CalculateNestedSize(child, (childSize) =>
        {
            // Учет якорей и относительного позиционирования
            Vector2 scaledSize = ApplyAnchorScaling(child, childSize);
            onChildSizeCalculated?.Invoke(scaledSize);
        });
    }

    private static Vector2 ApplyAnchorScaling(RectTransform child, Vector2 baseSize)
    {
        // Рассчитываем реальный вклад в размер родителя
        Rect parentRect = child.parent.GetComponent<RectTransform>().rect;
        Vector2 anchorMin = child.anchorMin;
        Vector2 anchorMax = child.anchorMax;

        // Рассчет относительного размера от якорей
        Vector2 anchorSize = new Vector2(
            parentRect.width * (anchorMax.x - anchorMin.x),
            parentRect.height * (anchorMax.y - anchorMin.y)
        );

        // Комбинируем с собственным размером
        return new Vector2(
            Mathf.Max(baseSize.x, anchorSize.x),
            Mathf.Max(baseSize.y, anchorSize.y)
        );
    }

    private static Vector2 CombineSizes(Vector2 parentSize, Vector2 childSize, RectTransform parent)
    {
        // Для разных типов компоновки потребуется разная логика,
        // здесь базовая реализация для демонстрации
        return new Vector2(
            Mathf.Max(parentSize.x, childSize.x),
            Mathf.Max(parentSize.y, childSize.y)
        );
    }
    private static IEnumerator HandleLayoutGroup(LayoutGroup group, System.Action<Vector2> callback)
    {
        Vector2 groupSize = Vector2.zero;
        List<Vector2> childSizes = new List<Vector2>();

        foreach (RectTransform child in group.transform)
        {
            yield return CalculateNestedSize(child, (childSize) => {
                childSizes.Add(childSize);
            });
        }

        // Расчет размера группы на основе типа
        if (group is VerticalLayoutGroup vertical)
        {
            groupSize = CalculateVerticalSize(vertical, childSizes);
        }
        else if (group is HorizontalLayoutGroup horizontal)
        {
            groupSize = CalculateHorizontalSize(horizontal, childSizes);
        }
        else if (group is GridLayoutGroup grid)
        {
            groupSize = CalculateGridSize(grid, childSizes);
        }

        callback?.Invoke(groupSize);
    }

    private static Vector2 CalculateVerticalSize(VerticalLayoutGroup group, List<Vector2> children)
    {
        float height = group.padding.vertical;
        float width = 0;

        foreach (var child in children)
        {
            height += child.y;
            width = Mathf.Max(width, child.x);
            height += group.spacing;
        }

        return new Vector2(
            width + group.padding.horizontal,
            height - (children.Count > 0 ? group.spacing : 0)
        );
    }

    private static Vector2 CalculateHorizontalSize(HorizontalLayoutGroup group, List<Vector2> children)
    {
        float width = group.padding.horizontal;
        float height = 0;

        foreach (var child in children)
        {
            width += child.x;
            height = Mathf.Max(height, child.y);
            width += group.spacing;
        }

        return new Vector2(
            width - (children.Count > 0 ? group.spacing : 0),
            height + group.padding.vertical
        );
    }

    private static Vector2 CalculateGridSize(GridLayoutGroup grid, List<Vector2> children)
    {
        int cellCount = children.Count;
        int cellsPerLine = grid.constraintCount;

        if (grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            cellsPerLine = grid.constraintCount;
        }
        else if (grid.constraint == GridLayoutGroup.Constraint.FixedRowCount)
        {
            cellsPerLine = Mathf.CeilToInt(cellCount / (float)grid.constraintCount);
        }

        Vector2 cellSize = grid.cellSize;
        Vector2 spacing = grid.spacing;

        float width = grid.padding.horizontal + (cellsPerLine * cellSize.x) + ((cellsPerLine - 1) * spacing.x);
        float height = grid.padding.vertical + (Mathf.CeilToInt(cellCount / (float)cellsPerLine) * cellSize.y)
                     + ((Mathf.CeilToInt(cellCount / (float)cellsPerLine) - 1) * spacing.y);

        return new Vector2(width, height);
    }

    private static bool TryGetContentSize(RectTransform target, out Vector2 size)
    {
        var fitter = target.GetComponent<ContentSizeFitter>();
        if (fitter == null)
        {
            size = Vector2.zero;
            return false;
        }

        size = target.rect.size;

        // Для ContentSizeFitter определяем размеры через дочерние элементы
        if (fitter.horizontalFit == ContentSizeFitter.FitMode.PreferredSize ||
            fitter.verticalFit == ContentSizeFitter.FitMode.PreferredSize)
        {
            // Здесь можно добавить дополнительные расчеты если нужно
            size = CalculateSimpleSize(target);
        }

        return true;
    }

    private static Vector2 CalculateSimpleSize(RectTransform target)
    {
        // Учитываем якоря и пивоты
        Vector2 size = target.rect.size;
        Vector2 anchorDelta = target.anchorMax - target.anchorMin;

        return new Vector2(
            size.x * Mathf.Abs(anchorDelta.x),
            size.y * Mathf.Abs(anchorDelta.y)
        );
    }
}