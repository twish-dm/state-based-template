using StateEngine.Events;
using StateEngine.Models;
using StateEngine.States;
using UnityEngine;

namespace StateEngine.Behaviours
{
    /// <summary>
    /// Local state machine scope. Child <see cref="DynamicBehaviour"/>s bind to it instead of the engine.
    /// </summary>
    public abstract class StaterBehaviour : MonoBehaviour, IStateContext
    {
        [SerializeField, Tooltip("Share the engine model instead of creating an isolated one.")]
        private bool m_UseEngineContext;

        private IModel m_Model;
        private Stater m_Stater;
        private bool m_OwnsModel;
        private bool m_IsInitialized;

        public IModel Model
        {
            get
            {
                EnsureInitialized();
                return m_Model;
            }
        }

        public IEventer Eventer => Model.Eventer;

        public IStater Stater
        {
            get
            {
                EnsureInitialized();
                return m_Stater;
            }
        }

        protected virtual void Awake() => EnsureInitialized();

        protected virtual void OnDestroy()
        {
            if (!m_IsInitialized)
                return;

            m_Stater.Dispose();

            if (m_OwnsModel)
                m_Model.Eventer.Dispose();
        }

        /// <summary>
        /// Register states and start the initial one here.
        /// </summary>
        protected abstract void OnInitialize();

        // Children may resolve this scope from their own Awake before ours has run.
        private void EnsureInitialized()
        {
            if (m_IsInitialized)
                return;

            m_IsInitialized = true;

            IEngine engine = ServiceRegistry.Get<IEngine>();
            m_OwnsModel = !m_UseEngineContext;
            m_Model = m_OwnsModel ? new DynamicModel(new Eventer()) : engine.Model;
            m_Stater = new Stater(m_Model, engine.PrimaryModel);

            OnInitialize();
        }
    }
}
