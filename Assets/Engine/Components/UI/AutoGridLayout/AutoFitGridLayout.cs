using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class AutoFitGridLayout : GridLayoutGroup
{
    public enum FitAxis { None, Width, Height }
    public FitAxis fitAxis = FitAxis.None;
    public int fitCount = 1;

    protected override void Start()
    {
        base.Start();
        UpdateCellSize();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        UpdateCellSize();
    }
#endif

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        UpdateCellSize();
    }

    private void UpdateCellSize()
    {
        if (fitAxis == FitAxis.None)
            return;

        RectTransform rect = rectTransform;
        if (rect == null || fitCount <= 0)
            return;

        if (fitAxis == FitAxis.Width)
        {
            constraint = Constraint.FixedColumnCount;
            constraintCount = fitCount;

            float totalWidth = rect.rect.width - padding.horizontal - (fitCount - 1) * spacing.x;
            float newWidth = Mathf.Max(0, totalWidth) / fitCount;
            cellSize = new Vector2(newWidth, cellSize.y);
        }
        else
        {
            constraint = Constraint.FixedRowCount;
            constraintCount = fitCount;

            float totalHeight = rect.rect.height - padding.vertical - (fitCount - 1) * spacing.y;
            float newHeight = Mathf.Max(0, totalHeight) / fitCount;
            cellSize = new Vector2(cellSize.x, newHeight);
        }
    }

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        UpdateCellSize();
    }

    public override void CalculateLayoutInputVertical()
    {
        base.CalculateLayoutInputVertical();
        UpdateCellSize();
    }
}
