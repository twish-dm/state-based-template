using StateEngine.Behaviours;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StateEngine.Components
{
    /// <summary>
    /// Displays a string model value in a TextMeshPro or legacy UI Text component.
    /// </summary>
    public sealed class TextBind : BindingBehaviour
    {
        private TMP_Text m_Text;
        private Text m_LegacyText;

        protected override string KeySuffix => "Text";

        protected override void OnInitialize()
        {
            if (!TryGetComponent(out m_Text) && !TryGetComponent(out m_LegacyText))
                Debug.LogWarning($"{nameof(TextBind)} on '{name}' has no text component to write to.", this);

            Eventer.Add<string>(Key, SetText);
        }

        private void Start()
        {
            SetText(Model.Get(Key, string.Empty));
        }

        protected override void OnRelease()
        {
            Eventer.Remove<string>(Key, SetText);
        }

        private void SetText(string value)
        {
            value ??= string.Empty;

            if (m_Text != null)
                m_Text.text = value;
            else if (m_LegacyText != null)
                m_LegacyText.text = value;
        }
    }
}
