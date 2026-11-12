using StateEngine.Behaviours;
using UnityEngine;

namespace StateEngine.Components
{
    /// <summary>
    /// Toggles the GameObject from a boolean model value.
    /// </summary>
    public sealed class ActivatorBind : BindingBehaviour
    {
        [SerializeField] private bool m_DefaultValue;
        [SerializeField] private bool m_Invert;

        protected override string KeySuffix => "Activator";

        protected override void OnInitialize()
        {
            Eventer.Add<bool>(Key, Apply);
        }

        private void Start()
        {
            Apply(Model.Get(Key, m_DefaultValue));
        }

        protected override void OnRelease()
        {
            Eventer.Remove<bool>(Key, Apply);
        }

        private void Apply(bool value) => gameObject.SetActive(value != m_Invert);
    }
}
