namespace StateEngine
{
    using StateEngine.Events;
    using StateEngine.Model;
    using StateEngine.States;

    using System;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.Events;

    [DefaultExecutionOrder(-800)]
    public class Engine : MonoBehaviour, IStateBehaviour
    {
        public const string KEY = "engine";
        private Dictionary<string, UnityEventBase> m_EventsData;
        protected DynamicModel model { get; set; }
        protected Eventer eventer { get; set; }
        protected Stater stater { get; set; }
        public IEventer Eventer => eventer;
        public IStater Stater => stater;
        public IModel Model => model;

        private void Awake()
        {
            if (StaticModel.ContainsKey(KEY))
                throw new Exception("Engine already running");
            m_EventsData = new Dictionary<string, UnityEventBase>();
            eventer = new Eventer(m_EventsData);
            model = new DynamicModel(eventer);
            stater = new Stater(model);
            StaticModel.Add(KEY, (IStateBehaviour)this);
        }
    }
}
