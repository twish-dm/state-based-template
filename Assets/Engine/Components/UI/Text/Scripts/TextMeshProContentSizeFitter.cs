using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(TextMeshProUGUI))]
[DisallowMultipleComponent]
[ExecuteAlways]
public class TextMeshProContentSizeFitter : MonoBehaviour
{
    [SerializeField] private bool m_FitToWidth = true;
    [SerializeField] private float m_MaxWidth; 
    public enum FitMode
    {
        Unconstrained,
        PreferredSize
    }

    [SerializeField] private FitMode horizontalFit = FitMode.PreferredSize;
    [SerializeField] private FitMode verticalFit = FitMode.PreferredSize;

    private TextMeshProUGUI textMeshPro;
    private RectTransform rectTransform;
    private Vector2 previousSize;
    private FontStyles previousFontStyle;
    private TMP_FontAsset previousFont;
    private float previousFontSize;

#if UNITY_EDITOR
    private void OnValidate()
    {
        EditorApplication.delayCall += _OnValidate;
    }

    private void _OnValidate()
    {
        if (this == null) return;
        AdjustSize(true);
    }
#endif

    private void Awake()
    {
        CacheComponents();
        CacheTextProperties();
    }

    private void OnEnable()
    {
        CacheComponents();
#if UNITY_EDITOR
        EditorApplication.update += EditorUpdate;
#endif
        AdjustSize(true);
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.update -= EditorUpdate;
#endif
    }

    private void Update()
    {
        CheckForChanges();
    }

#if UNITY_EDITOR
    private void EditorUpdate()
    {
        if (!EditorApplication.isPlaying)
        {
            CheckForChanges();
        }
    }
#endif

    private void CacheComponents()
    {
        if (textMeshPro == null) textMeshPro = GetComponent<TextMeshProUGUI>();
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
    }

    private void CacheTextProperties()
    {
        if (textMeshPro != null)
        {
            previousFont = textMeshPro.font;
            previousFontSize = textMeshPro.fontSize;
            previousFontStyle = textMeshPro.fontStyle;
            previousSize = GetPreferredSize();
        }
    }

    private bool CheckForChanges()
    {
        bool hasChanged = false;

        if (textMeshPro == null || rectTransform == null)
            return false;

        // Проверка изменений свойств шрифта
        if (previousFont != textMeshPro.font ||
            previousFontSize != textMeshPro.fontSize ||
            previousFontStyle != textMeshPro.fontStyle)
        {
            hasChanged = true;
            CacheTextProperties();
        }

        // Проверка изменения размеров текста
        Vector2 currentSize = GetPreferredSize();
        if (currentSize != previousSize)
        {
            hasChanged = true;
            previousSize = currentSize;
        }

        if (hasChanged)
        {
            AdjustSize();
        }

        return hasChanged;
    }

    private Vector2 GetPreferredSize()
    {
        if (textMeshPro == null) return Vector2.zero;

        textMeshPro.ForceMeshUpdate(true, true);
        return new Vector2(
            Mathf.Min(textMeshPro.preferredWidth, this.m_FitToWidth?m_MaxWidth: textMeshPro.preferredWidth),
            textMeshPro.preferredHeight
        );
    }

    public void AdjustSize(bool forceUpdate = false)
    {
        if (textMeshPro == null || rectTransform == null)
            return;

        Vector2 preferredSize = GetPreferredSize();

        if (horizontalFit == FitMode.PreferredSize || forceUpdate)
        {
            rectTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal,
                preferredSize.x
            );
        }

        if (verticalFit == FitMode.PreferredSize || forceUpdate)
        {
            rectTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical,
                preferredSize.y
            );
        }
    }
}