namespace StateEngine.Behaviours
{
    using StateEngine.Model;
    using StateEngine.Events;
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using StateEngine.States;

    abstract public class DynamicBehaviour : MonoBehaviour, IDisposable, IInitialize
    {
        [SerializeField] private bool m_IsGlobal;
        protected event UnityAction onUpdate, onFixedUpdate, onLateUpdate;
        private IModel m_Model;
        private IEventer m_Eventer;

        virtual protected void Awake()
        {
            if (!IsInitialized)
            {
                m_Model = m_IsGlobal ? StaticModel.Get<IStateBehaviour>(Engine.KEY).Model : GetComponentInParent<IStateBehaviour>().Model ?? StaticModel.Get<IStateBehaviour>(Engine.KEY).Model;
                m_Eventer = m_IsGlobal ? StaticModel.Get<IStateBehaviour>(Engine.KEY).Eventer : GetComponentInParent<IStateBehaviour>().Eventer ?? StaticModel.Get<IStateBehaviour>(Engine.KEY).Eventer;
                m_Stater = m_IsGlobal ? StaticModel.Get<IStateBehaviour>(Engine.KEY).Stater : GetComponentInParent<IStateBehaviour>().Stater ?? StaticModel.Get<IStateBehaviour>(Engine.KEY).Stater;
                Debug.Log($"DynamicBehaviour[{name}] Initialize");
                Initialize();
                IsInitialized = true;
            }
        }

        virtual protected IModel model => m_Model;
        virtual protected IEventer eventer => m_Eventer;
        private IStater m_Stater;
        virtual public void Send(string type)
        {
            m_Stater.Invoke(type);
        }
        virtual public void Send<T>(string type, T eventData) where T : IEventData
        {
            m_Stater.Invoke(type, eventData);
        }
        private void Update()
        {
            if (IsDestroyed) return;
            onUpdate?.Invoke();
        }
        private void FixedUpdate()
        {
            if (IsDestroyed) return;
            onFixedUpdate?.Invoke();
        }
        private void LateUpdate()
        {
            if (IsDestroyed) return;
            onLateUpdate?.Invoke();
        }

        virtual public bool IsDestroyed { get; protected set; }

        public bool IsInitialized { get; protected set; }

        virtual public void Dispose()
        {
            if (IsDestroyed) return;
            gameObject.SetActive(false);
            Destroy(gameObject);
            IsDestroyed = true;
        }

        virtual public void Initialize()
        {
        }
    }
}
