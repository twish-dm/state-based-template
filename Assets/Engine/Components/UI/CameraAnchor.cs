using UnityEngine;

namespace StateEngine.Components.UI
{
    /// <summary>
    /// Fits the camera viewport into an anchored rect, the same way a RectTransform fits its parent.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public sealed class CameraAnchor : MonoBehaviour
    {
        private const float k_MinViewportSize = 0.0001f;

        [SerializeField] private Vector2 m_AnchorMin = new Vector2(0f, 0f);
        [SerializeField] private Vector2 m_AnchorMax = new Vector2(1f, 1f);

        [SerializeField, Tooltip("Offset of the lower-left corner in pixels.")]
        private Vector2 m_OffsetMin;

        [SerializeField, Tooltip("Offset of the upper-right corner in pixels, positive values shrink the viewport.")]
        private Vector2 m_OffsetMax;

        private Camera m_Camera;
        private Vector2Int m_LastScreenSize;

        private void OnEnable()
        {
            m_Camera = GetComponent<Camera>();
            Apply();
        }

        private void Update()
        {
            var screenSize = new Vector2Int(Screen.width, Screen.height);
            if (screenSize != m_LastScreenSize)
                Apply();
        }

        private void OnValidate()
        {
            if (m_Camera != null)
                Apply();
        }

        [ContextMenu("Apply")]
        public void Apply()
        {
            m_LastScreenSize = new Vector2Int(Screen.width, Screen.height);
            if (m_LastScreenSize.x == 0 || m_LastScreenSize.y == 0)
                return;

            Vector2 screen = m_LastScreenSize;
            Vector2 min = Clamp01(m_AnchorMin + m_OffsetMin / screen);
            Vector2 max = Clamp01(m_AnchorMax - m_OffsetMax / screen);

            m_Camera.rect = new Rect(
                min.x,
                min.y,
                Mathf.Max(k_MinViewportSize, max.x - min.x),
                Mathf.Max(k_MinViewportSize, max.y - min.y));
        }

        private static Vector2 Clamp01(Vector2 value) => new Vector2(Mathf.Clamp01(value.x), Mathf.Clamp01(value.y));
    }
}
