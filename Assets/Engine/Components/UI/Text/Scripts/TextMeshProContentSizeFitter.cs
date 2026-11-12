using TMPro;
using UnityEngine;

namespace StateEngine.Components.UI
{
    /// <summary>
    /// Resizes the rect to the preferred size of its text, optionally wrapping at a maximum width.
    /// Reacts to TextMeshPro change events instead of polling every frame.
    /// </summary>
    [ExecuteAlways, DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class TextMeshProContentSizeFitter : MonoBehaviour
    {
        [SerializeField] private bool m_FitWidth = true;
        [SerializeField] private bool m_FitHeight = true;

        [SerializeField, Min(0f), Tooltip("Text wraps at this width. Zero means unlimited.")]
        private float m_MaxWidth;

        private TextMeshProUGUI m_Text;
        private RectTransform m_RectTransform;

        private void OnEnable()
        {
            m_Text = GetComponent<TextMeshProUGUI>();
            m_RectTransform = (RectTransform)transform;

            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
            Refresh();
        }

        private void OnDisable()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null && isActiveAndEnabled)
                    Refresh();
            };
        }
#endif

        [ContextMenu("Refresh")]
        public void Refresh()
        {
            if (m_Text == null)
                return;

            float widthLimit = m_MaxWidth > 0f ? m_MaxWidth : float.PositiveInfinity;
            Vector2 preferred = m_Text.GetPreferredValues(m_Text.text, widthLimit, float.PositiveInfinity);

            if (m_FitWidth)
                SetSize(RectTransform.Axis.Horizontal, Mathf.Min(preferred.x, widthLimit));

            if (m_FitHeight)
                SetSize(RectTransform.Axis.Vertical, preferred.y);
        }

        private void OnTextChanged(Object changed)
        {
            if (changed == m_Text)
                Refresh();
        }

        private void SetSize(RectTransform.Axis axis, float size)
        {
            float current = axis == RectTransform.Axis.Horizontal ? m_RectTransform.rect.width : m_RectTransform.rect.height;
            if (!Mathf.Approximately(current, size))
                m_RectTransform.SetSizeWithCurrentAnchors(axis, size);
        }
    }
}
