namespace StateEngine.Behaviours
{
    using StateEngine.Model;
    using StateEngine.Events;
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using StateEngine.States;

    abstract public class DynamicBehaviour : MonoBehaviour, IInitialize
    {
        [SerializeField] protected bool m_GetFromEngine;
        protected event UnityAction onUpdate, onFixedUpdate, onLateUpdate;
        private IModel m_Model;
        private IEventer m_Eventer;

        virtual protected void Awake()
        {
            if (!IsInitialized)
            {
                IStaterBehaviour parentStater = GetComponentInParent<IStaterBehaviour>();

                if (!m_GetFromEngine && parentStater != null)
                {
                    m_Model = parentStater.Model;
                    m_Eventer = parentStater.Eventer;
                    m_Stater = parentStater.Stater;
                }
                else
                {
                    m_GetFromEngine = true;
                    m_Model = StaticModel.Get<IEngine>(Engine.KEY).Model;
                    m_Eventer = StaticModel.Get<IEngine>(Engine.KEY).Model.Eventer;
                    m_Stater = StaticModel.Get<IEngine>(Engine.KEY).Stater;
                }
                Debug.Log($"DynamicBehaviour[{name}] Initialize");
                Initialize();
                IsInitialized = true;
            }
        }

        virtual protected IModel internalModel => m_Model;
        
        virtual protected IEventer eventer => m_Eventer;
        private IStater m_Stater;
        virtual public void Send(string type)
        {
            m_Stater.Invoke(type);
        }
        virtual public void Send<T>(string type, T eventData)
        {
            Debug.Log(name+"=> " + m_Stater);
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

        bool IInitialize.IsInitialized => throw new NotImplementedException();

        virtual public void Dispose()
        {
            if (IsDestroyed) return;
            gameObject.SetActive(false);
            Destroy(gameObject);
            IsDestroyed = true;
        }

        abstract public void Initialize();


    }
}
