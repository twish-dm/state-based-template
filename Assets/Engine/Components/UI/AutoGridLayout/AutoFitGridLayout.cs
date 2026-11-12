using UnityEngine;
using UnityEngine.UI;

namespace StateEngine.Components.UI
{
    /// <summary>
    /// Grid layout that stretches cells to fit a fixed number of columns or rows into the available space.
    /// </summary>
    [ExecuteAlways]
    public class AutoFitGridLayout : GridLayoutGroup
    {
        public enum FitAxis
        {
            None,
            Width,
            Height
        }

        [SerializeField] private FitAxis m_FitAxis = FitAxis.None;
        [SerializeField, Min(1)] private int m_FitCount = 1;

        public FitAxis Fit
        {
            get => m_FitAxis;
            set
            {
                m_FitAxis = value;
                SetDirty();
            }
        }

        public int FitCount
        {
            get => m_FitCount;
            set
            {
                m_FitCount = Mathf.Max(1, value);
                SetDirty();
            }
        }

        public override void CalculateLayoutInputHorizontal()
        {
            UpdateCellSize();
            base.CalculateLayoutInputHorizontal();
        }

        public override void CalculateLayoutInputVertical()
        {
            UpdateCellSize();
            base.CalculateLayoutInputVertical();
        }

        private void UpdateCellSize()
        {
            if (m_FitAxis == FitAxis.None)
                return;

            Rect rect = rectTransform.rect;
            constraintCount = m_FitCount;

            if (m_FitAxis == FitAxis.Width)
            {
                constraint = Constraint.FixedColumnCount;
                float available = rect.width - padding.horizontal - spacing.x * (m_FitCount - 1);
                cellSize = new Vector2(Mathf.Max(0f, available) / m_FitCount, cellSize.y);
            }
            else
            {
                constraint = Constraint.FixedRowCount;
                float available = rect.height - padding.vertical - spacing.y * (m_FitCount - 1);
                cellSize = new Vector2(cellSize.x, Mathf.Max(0f, available) / m_FitCount);
            }
        }
    }
}
