using System;
using StateEngine.Events;
using StateEngine.Models;
using StateEngine.States;
using UnityEngine;

namespace StateEngine.Behaviours
{
    /// <summary>
    /// Base for components that read and write a model. Binds to the nearest <see cref="IStateContext"/>
    /// up the hierarchy, or to the engine if there is none.
    /// </summary>
    public abstract class DynamicBehaviour : MonoBehaviour, IDisposable
    {
        [SerializeField, Tooltip("Bind to the engine context even if a parent state machine exists.")]
        private bool m_UseEngineContext;

        private IStateContext m_Context;

        public bool IsInitialized { get; private set; }

        protected IModel Model => m_Context.Model;

        protected IEventer Eventer => m_Context.Eventer;

        protected IStater Stater => m_Context.Stater;

        protected virtual void Awake()
        {
            m_Context = ResolveContext();
            IsInitialized = true;
            OnInitialize();
        }

        protected virtual void OnDestroy()
        {
            if (!IsInitialized)
                return;

            OnRelease();
            IsInitialized = false;
        }

        public void Dispose() => Destroy(gameObject);

        /// <summary>
        /// Called once from Awake after the context is resolved. Subscribe to the model here.
        /// </summary>
        protected abstract void OnInitialize();

        /// <summary>
        /// Called from OnDestroy. Undo everything done in <see cref="OnInitialize"/>.
        /// </summary>
        protected virtual void OnRelease()
        {
        }

        protected void Send(string type) => Stater.Invoke(type);

        protected void Send<T>(string type, T data) => Stater.Invoke(type, data);

        private IStateContext ResolveContext()
        {
            if (!m_UseEngineContext)
            {
                IStateContext parent = GetComponentInParent<IStateContext>();
                if (parent != null)
                    return parent;
            }

            return ServiceRegistry.Get<IEngine>();
        }
    }
}
