using StateEngine.Behaviours;
using UnityEngine;
using UnityEngine.UI;

namespace StateEngine.Components
{
    /// <summary>
    /// Sends <see cref="BindingBehaviour.Key"/> to the active state when the button is clicked.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public sealed class ButtonBind : BindingBehaviour
    {
        private Button m_Button;

        protected override string KeySuffix => "Button";

        protected override void OnInitialize()
        {
            m_Button = GetComponent<Button>();
            m_Button.onClick.AddListener(OnClick);
        }

        protected override void OnRelease()
        {
            m_Button.onClick.RemoveListener(OnClick);
        }

        private void OnClick() => Send(Key);
    }
}
