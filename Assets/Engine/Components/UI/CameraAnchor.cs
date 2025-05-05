using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAnchor : MonoBehaviour
{
    public Camera targetCamera;
    [Tooltip("Normalized minimum anchors (lower-left corner)")]
    public Vector2 anchorMin = new Vector2(0.5f, 0.5f);
    [Tooltip("Normalized maximum anchors (upper-right corner)")]
    public Vector2 anchorMax = new Vector2(0.5f, 0.5f);
    [Tooltip("Pixel offset from minimum anchors")]
    public Vector2 offsetMin = Vector2.zero;
    [Tooltip("Pixel offset from maximum anchors")]
    public Vector2 offsetMax = Vector2.zero;

    private Vector2 cachedScreenSize;
    private Vector2 cachedAnchorMin;
    private Vector2 cachedAnchorMax;
    private Vector2 cachedOffsetMin;
    private Vector2 cachedOffsetMax;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = GetComponent<Camera>();

        UpdateViewport(true);
    }

    private void Update()
    {
        UpdateViewport();
    }

    private void UpdateViewport(bool forceUpdate = false)
    {
        if (targetCamera == null)
            return;

        // Защита от нулевого размера экрана
        if (Screen.width == 0 || Screen.height == 0)
            return;

        bool screenSizeChanged =
            Screen.width != cachedScreenSize.x ||
            Screen.height != cachedScreenSize.y;

        bool settingsChanged =
            !Mathf.Approximately(anchorMin.x, cachedAnchorMin.x) ||
            !Mathf.Approximately(anchorMin.y, cachedAnchorMin.y) ||
            !Mathf.Approximately(anchorMax.x, cachedAnchorMax.x) ||
            !Mathf.Approximately(anchorMax.y, cachedAnchorMax.y) ||
            offsetMin != cachedOffsetMin ||
            offsetMax != cachedOffsetMax;

        if (!forceUpdate && !screenSizeChanged && !settingsChanged)
            return;

        // Обновляем кэш
        cachedScreenSize = new Vector2(Screen.width, Screen.height);
        cachedAnchorMin = anchorMin;
        cachedAnchorMax = anchorMax;
        cachedOffsetMin = offsetMin;
        cachedOffsetMax = offsetMax;

        // Конвертируем смещения с защитой от деления на ноль
        Vector2 offsetMinNorm = new Vector2(
            offsetMin.x / cachedScreenSize.x,
            offsetMin.y / cachedScreenSize.y
        );

        Vector2 offsetMaxNorm = new Vector2(
            offsetMax.x / cachedScreenSize.x,
            offsetMax.y / cachedScreenSize.y
        );

        // Рассчитываем границы с дополнительными проверками
        float xMin = Mathf.Clamp01(anchorMin.x + offsetMinNorm.x);
        float yMin = Mathf.Clamp01(anchorMin.y + offsetMinNorm.y);
        float xMax = Mathf.Clamp01(anchorMax.x - offsetMaxNorm.x);
        float yMax = Mathf.Clamp01(anchorMax.y - offsetMaxNorm.y);

        // Корректируем границы
        if (xMin > xMax) xMax = xMin + 0.0001f;
        if (yMin > yMax) yMax = yMin + 0.0001f;

        // Применяем изменения
        targetCamera.rect = new Rect(
            xMin,
            yMin,
            Mathf.Max(0.0001f, xMax - xMin),
            Mathf.Max(0.0001f, yMax - yMin)
        );
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (targetCamera == null)
            targetCamera = GetComponent<Camera>();

        UpdateViewport(true);
    }
#endif
}