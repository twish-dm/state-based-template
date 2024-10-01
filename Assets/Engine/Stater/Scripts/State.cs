namespace StateEngine.States
{
    using StateEngine.Model;
    using StateEngine.Events;
    using UnityEngine.Events;
    using System.Collections.Generic;
    using UnityEngine;
    using StateEngine.Views;

    abstract public class State : IState
    {

        virtual public IModel InternalModel { get; set; }
        private Dictionary<string, UnityEventBase> m_EventsData;
        private Eventer m_Eventer;
        virtual public IEventer Eventer { get => m_Eventer; }
        virtual public IStater Stater { get; set; }

        virtual public string Name => GetType().Name;

        virtual public bool IsInitialized { get; private set; }
        virtual public IViewer Viewer { get; set; }
        public IModel MainModel { get; set; }

        virtual public void Dispose()
        {
            Eventer.Dispose();
        }

        abstract public void Enter();

        abstract public void Exit();

        virtual public void Initialize()
        {
            if (IsInitialized) return;
            Debug.Log($"State[{Name}] initialized");
            IsInitialized = true;
            m_EventsData = new Dictionary<string, UnityEventBase>();
            m_Eventer = new Eventer(m_EventsData);
        }

        public void Send(string type)
        {
            InternalModel.Eventer.Invoke(type);
        }

        public void Send<T>(string type, T eventData) where T : IEventData
        {
            InternalModel.Eventer.Invoke(type, eventData);
        }
    }
}
