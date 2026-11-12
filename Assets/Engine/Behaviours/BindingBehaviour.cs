using UnityEngine;

namespace StateEngine.Behaviours
{
    /// <summary>
    /// Behaviour bound to a single model key. Falls back to "{GameObject name}{suffix}" when the key is empty.
    /// </summary>
    public abstract class BindingBehaviour : DynamicBehaviour
    {
        [SerializeField, Tooltip("Model key or event name. Leave empty to use the default derived from the object name.")]
        private string m_Key;

        protected string Key => string.IsNullOrEmpty(m_Key) ? DefaultKey : m_Key;

        protected abstract string KeySuffix { get; }

        private string DefaultKey => name + KeySuffix;

        protected virtual void Reset()
        {
            m_Key = DefaultKey;
        }
    }
}
