
namespace StateEngine.Behaviours
{
    using StateEngine.Events;
    using StateEngine.Model;
    using StateEngine.States;

    using System;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.Events;

    abstract public class StaterBehaviour : MonoBehaviour, IStaterBehaviour, IInitialize
    {
        [SerializeField] private bool m_GetFromEngine;

        private Dictionary<string, UnityEventBase> m_EventsData;
        protected IModel model { get; set; }
        protected IEventer eventer { get; set; }
        protected Stater stater { get; set; }
        protected IEngine primaryStater { get; set; }
        public IEventer Eventer => eventer;
        public IStater Stater => stater;
        public IModel Model => model;
        public IEngine PrimaryStater => primaryStater;
        public IStaterBehaviour ParentStaterBehaviour { get; protected set; }

        virtual protected void Awake()
        {
            if (!IsInitialized)
            {
                if (primaryStater == null)
                    primaryStater = StaticModel.Get<IEngine>(Engine.KEY);
                ParentStaterBehaviour = transform.parent.GetComponentInParent<StaterBehaviour>();
                Debug.Log($"{name} => {ParentStaterBehaviour}");
                m_EventsData = null;

                if (m_GetFromEngine)
                {
                    eventer = primaryStater.Eventer;
                    model = primaryStater.Model;
                }
                else
                {
                    m_EventsData = new Dictionary<string, UnityEventBase>();
                    eventer = new Eventer(m_EventsData);
                    model = new DynamicModel(eventer);
                }
                stater = new Stater(model, primaryStater.PrimaryModel);

                Debug.Log($"StaterBehaviour[{name}] Initialize");
                IsInitialized = true;

                Initialize();
            }
        }

        virtual public void Send(string type)
        {
            stater.Invoke(type);
        }
        virtual public void Send<T>(string type, T eventData)
        {
            stater.Invoke(type, eventData);
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

        abstract public void Initialize();
    }
}
