using System.Collections;

using UnityEngine;

namespace Engine.Components.UI
{
    using UnityEngine;

    [RequireComponent(typeof(RectTransform)),ExecuteAlways]
    public class AspectRatioContentSwitcher : MonoBehaviour
    {
        [SerializeField, Range(0, 2f)] private float m_AspectRatio = 1f;
        [Header("Content References")]
        [Tooltip("Content to show in vertical orientations (height > width)")]
        [SerializeField] protected GameObject verticalContent;

        [Tooltip("Content to show in horizontal orientations (width > height)")]
        [SerializeField] protected GameObject horizontalContent;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            CheckAspectRatio();
        }

        private void OnRectTransformDimensionsChange()
        {
            CheckAspectRatio();
        }

        private void CheckAspectRatio()
        {
            if (_rectTransform == null) return;

            float width = _rectTransform.rect.width;
            float height = _rectTransform.rect.height;

            // Handle cases with zero size to prevent division by zero
            if (Mathf.Approximately(height, 0)) return;

            float aspectRatio = width / height;

            bool isHorizontal = aspectRatio > m_AspectRatio;

            SetContentActiveState(isHorizontal);
        }

        private void SetContentActiveState(bool isHorizontal)
        {
            if (horizontalContent != null)
                horizontalContent.SetActive(isHorizontal);

            if (verticalContent != null)
                verticalContent.SetActive(!isHorizontal);
        }

        // Для отладки/ручного обновления в редакторе
        [ContextMenu("Refresh Aspect Ratio Check")]
        public void RefreshManual()
        {
            CheckAspectRatio();
        }
    }
}