using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaterEngine.Components.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;

    [ExecuteInEditMode]
    public class FlexibleHorizontalLayoutGroup : LayoutGroup
    {
        public float Spacing = 0f;

        private readonly List<List<RectTransform>> m_Rows = new List<List<RectTransform>>();
        private readonly List<float> m_RowHeights = new List<float>();

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            CalculateRows();

            float totalMinWidth = GetTotalRowWidth(true);
            float totalPreferredWidth = GetTotalRowWidth(false);

            SetLayoutInputForAxis(totalMinWidth, totalPreferredWidth, 1, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            float totalHeight = padding.vertical;
            foreach (var height in m_RowHeights)
            {
                totalHeight += height + Spacing;
            }
            if (m_RowHeights.Count > 0) totalHeight -= Spacing;

            SetLayoutInputForAxis(totalHeight, totalHeight, 1, 1);
        }

        private void CalculateRows()
        {
            m_Rows.Clear();
            m_RowHeights.Clear();

            List<RectTransform> currentRow = new List<RectTransform>();
            float availableWidth = rectTransform.rect.width - padding.horizontal;
            float rowWidth = 0f;

            foreach (RectTransform child in rectChildren)
            {
                if (!child.gameObject.activeSelf) continue;

                float minWidth = LayoutUtility.GetMinWidth(child);
                float preferredWidth = LayoutUtility.GetPreferredWidth(child);
                float requiredWidth = Mathf.Max(minWidth, preferredWidth);

                // Проверка на перенос
                if (currentRow.Count > 0 &&
                    rowWidth + Spacing + minWidth > availableWidth)
                {
                    FinalizeRow(currentRow);
                    currentRow = new List<RectTransform>();
                    rowWidth = 0f;
                }

                // Добавляем элемент в строку (даже если он не влезает)
                if (currentRow.Count == 0)
                {
                    currentRow.Add(child);
                    rowWidth += requiredWidth;
                }
                else
                {
                    currentRow.Add(child);
                    rowWidth += Spacing + requiredWidth;
                }
            }

            if (currentRow.Count > 0)
            {
                FinalizeRow(currentRow);
            }
        }

        private void FinalizeRow(List<RectTransform> row)
        {
            m_Rows.Add(new List<RectTransform>(row));

            // Рассчет высоты строки
            float maxHeight = 0f;
            foreach (var child in row)
            {
                maxHeight = Mathf.Max(maxHeight, LayoutUtility.GetPreferredHeight(child));
            }
            m_RowHeights.Add(maxHeight);
        }

        private float GetTotalRowWidth(bool useMinWidth)
        {
            float maxWidth = 0f;
            foreach (var row in m_Rows)
            {
                float width = 0f;
                for (int i = 0; i < row.Count; i++)
                {
                    var child = row[i];
                    width += useMinWidth ?
                        LayoutUtility.GetMinWidth(child) :
                        LayoutUtility.GetPreferredWidth(child);

                    if (i > 0) width += Spacing;
                }
                maxWidth = Mathf.Max(maxWidth, width);
            }
            return maxWidth + padding.horizontal;
        }

        public override void SetLayoutHorizontal()
        {
            SetRows();
        }

        public override void SetLayoutVertical()
        {
            SetRowsVertical();
        }

        private void SetRows()
        {
            float availableWidth = rectTransform.rect.width - padding.horizontal;

            for (int i = 0; i < m_Rows.Count; i++)
            {
                var row = m_Rows[i];
                float rowWidth = GetRowWidth(row, false);
                float startX = GetStartOffset(0, rowWidth);

                float xPos = startX + padding.left;
                foreach (var child in row)
                {
                    float width = LayoutUtility.GetPreferredWidth(child);
                    SetChildAlongAxis(child, 0, xPos, width);
                    xPos += width + Spacing;
                }
            }
        }

        private void SetRowsVertical()
        {
            float yPos = padding.top;
            for (int i = 0; i < m_Rows.Count; i++)
            {
                float rowHeight = m_RowHeights[i];
                foreach (var child in m_Rows[i])
                {
                    SetChildAlongAxis(child, 1, yPos, rowHeight);
                }
                yPos += rowHeight + Spacing;
            }
        }

        private float GetRowWidth(List<RectTransform> row, bool useMinWidth)
        {
            float width = 0f;
            for (int i = 0; i < row.Count; i++)
            {
                var child = row[i];
                width += useMinWidth ?
                    LayoutUtility.GetMinWidth(child) :
                    LayoutUtility.GetPreferredWidth(child);

                if (i > 0) width += Spacing;
            }
            return width;
        }

        private float GetStartOffset(int axis, float requiredSpace)
        {
            float containerSize = rectTransform.rect.size[axis];
            float availableSpace = containerSize - padding.horizontal;
            return childAlignment switch
            {
                TextAnchor.UpperLeft or TextAnchor.MiddleLeft or TextAnchor.LowerLeft
                    => padding.left,
                TextAnchor.UpperCenter or TextAnchor.MiddleCenter or TextAnchor.LowerCenter
                    => (availableSpace - requiredSpace) * 0.5f + padding.left,
                TextAnchor.UpperRight or TextAnchor.MiddleRight or TextAnchor.LowerRight
                    => availableSpace - requiredSpace + padding.left,
                _ => 0
            };
        }
    }
}
