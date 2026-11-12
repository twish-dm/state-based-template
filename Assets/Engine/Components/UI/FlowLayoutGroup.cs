using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StateEngine.Components.UI
{
    /// <summary>
    /// Lays children out left to right and wraps them onto a new row when the width runs out.
    /// Horizontal alignment is taken from <see cref="LayoutGroup.childAlignment"/>.
    /// </summary>
    [ExecuteAlways]
    public sealed class FlowLayoutGroup : LayoutGroup
    {
        [SerializeField] private Vector2 m_Spacing;

        private readonly List<Row> m_Rows = new List<Row>();

        public Vector2 Spacing
        {
            get => m_Spacing;
            set => SetProperty(ref m_Spacing, value);
        }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            BuildRows();

            float minWidth = 0f;
            float preferredWidth = 0f;

            foreach (RectTransform child in rectChildren)
            {
                minWidth = Mathf.Max(minWidth, LayoutUtility.GetMinWidth(child));
                preferredWidth += LayoutUtility.GetPreferredWidth(child);
            }

            preferredWidth += m_Spacing.x * Mathf.Max(0, rectChildren.Count - 1);
            SetLayoutInputForAxis(minWidth + padding.horizontal, preferredWidth + padding.horizontal, -1f, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            float height = padding.vertical;
            for (int i = 0; i < m_Rows.Count; i++)
                height += m_Rows[i].Height;

            height += m_Spacing.y * Mathf.Max(0, m_Rows.Count - 1);
            SetLayoutInputForAxis(height, height, -1f, 1);
        }

        public override void SetLayoutHorizontal()
        {
            float availableWidth = rectTransform.rect.width - padding.horizontal;

            foreach (Row row in m_Rows)
            {
                float x = padding.left + (availableWidth - row.Width) * GetHorizontalAlignment();

                for (int i = row.Start; i < row.End; i++)
                {
                    RectTransform child = rectChildren[i];
                    float width = LayoutUtility.GetPreferredWidth(child);
                    SetChildAlongAxis(child, 0, x, width);
                    x += width + m_Spacing.x;
                }
            }
        }

        public override void SetLayoutVertical()
        {
            float y = padding.top;

            foreach (Row row in m_Rows)
            {
                for (int i = row.Start; i < row.End; i++)
                    SetChildAlongAxis(rectChildren[i], 1, y, row.Height);

                y += row.Height + m_Spacing.y;
            }
        }

        private void BuildRows()
        {
            m_Rows.Clear();

            float availableWidth = rectTransform.rect.width - padding.horizontal;
            var row = new Row();

            for (int i = 0; i < rectChildren.Count; i++)
            {
                RectTransform child = rectChildren[i];
                float width = LayoutUtility.GetPreferredWidth(child);
                float widthWithSpacing = row.Count > 0 ? width + m_Spacing.x : width;

                if (row.Count > 0 && row.Width + widthWithSpacing > availableWidth)
                {
                    m_Rows.Add(row);
                    row = new Row { Start = i };
                    widthWithSpacing = width;
                }

                row.End = i + 1;
                row.Width += widthWithSpacing;
                row.Height = Mathf.Max(row.Height, LayoutUtility.GetPreferredHeight(child));
            }

            if (row.Count > 0)
                m_Rows.Add(row);
        }

        private float GetHorizontalAlignment() => (int)childAlignment % 3 * 0.5f;

        private struct Row
        {
            public int Start;
            public int End;
            public float Width;
            public float Height;

            public int Count => End - Start;
        }
    }
}
