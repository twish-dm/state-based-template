using UnityEngine;

namespace StateEngine.Components.UI
{
    /// <summary>
    /// Switches between landscape and portrait content depending on the aspect ratio of this rect.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public sealed class AspectRatioContentSwitcher : MonoBehaviour
    {
        [SerializeField, Range(0.1f, 3f), Tooltip("Width / height ratio above which the horizontal content is shown.")]
        private float m_Threshold = 1f;

        [SerializeField, Tooltip("Shown when width / height is at or below the threshold.")]
        private GameObject m_VerticalContent;

        [SerializeField, Tooltip("Shown when width / height is above the threshold.")]
        private GameObject m_HorizontalContent;

        private RectTransform m_RectTransform;

        private void Awake()
        {
            m_RectTransform = (RectTransform)transform;
        }

        private void OnEnable() => Refresh();

        private void OnRectTransformDimensionsChange() => Refresh();

        [ContextMenu("Refresh")]
        public void Refresh()
        {
            if (m_RectTransform == null)
                return;

            Rect rect = m_RectTransform.rect;
            if (rect.height <= 0f)
                return;

            bool isHorizontal = rect.width / rect.height > m_Threshold;

            if (m_HorizontalContent != null)
                m_HorizontalContent.SetActive(isHorizontal);

            if (m_VerticalContent != null)
                m_VerticalContent.SetActive(!isHorizontal);
        }
    }
}
