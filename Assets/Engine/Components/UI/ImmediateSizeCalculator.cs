using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StateEngine.Components.UI
{
    /// <summary>
    /// Resizes the rect to its content in the same frame: forces a layout rebuild of the hierarchy,
    /// then freezes nested layout components while the bounds are measured.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public sealed class ImmediateSizeCalculator : MonoBehaviour
    {
        [SerializeField] private RectOffset m_Padding = new RectOffset();

        private readonly List<Behaviour> m_DisabledLayouts = new List<Behaviour>();

        private void OnEnable() => Recalculate();

        [ContextMenu("Recalculate")]
        public void Recalculate()
        {
            var rectTransform = (RectTransform)transform;

            RebuildRecursive(rectTransform);
            DisableLayoutComponents();

            try
            {
                Vector2 contentSize = rectTransform.GetChildrenBounds().size;
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentSize.x + m_Padding.horizontal);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentSize.y + m_Padding.vertical);
            }
            finally
            {
                RestoreLayoutComponents();
            }
        }

        private static void RebuildRecursive(RectTransform root)
        {
            for (int i = root.childCount - 1; i >= 0; i--)
            {
                if (root.GetChild(i) is RectTransform child)
                    RebuildRecursive(child);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(root);
        }

        private void DisableLayoutComponents()
        {
            m_DisabledLayouts.Clear();

            foreach (Behaviour behaviour in GetComponentsInChildren<Behaviour>())
            {
                if (behaviour.enabled && (behaviour is LayoutGroup || behaviour is ContentSizeFitter))
                {
                    behaviour.enabled = false;
                    m_DisabledLayouts.Add(behaviour);
                }
            }
        }

        private void RestoreLayoutComponents()
        {
            foreach (Behaviour behaviour in m_DisabledLayouts)
            {
                if (behaviour != null)
                    behaviour.enabled = true;
            }

            m_DisabledLayouts.Clear();
        }
    }
}
