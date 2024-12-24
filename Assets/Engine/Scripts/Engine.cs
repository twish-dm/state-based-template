namespace StateEngine
{
    using StateEngine.Events;
    using StateEngine.Model;
    using StateEngine.States;

    using System;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.Events;

    [DefaultExecutionOrder(-1200)]
    public class Engine : MonoBehaviour, IEngine
    {
        public const string KEY = "engine";
        private Dictionary<string, UnityEventBase> m_InternalEventsData, m_MainEventsData;
        protected DynamicModel internalModel { get; set; }
        protected DynamicModel primaryModel { get; set; }
        protected Eventer internalEventer { get; set; }
        protected Eventer primaryEventer { get; set; }
        protected Stater stater { get; set; }
        public IEventer Eventer => internalEventer;
        public IStater Stater => stater;
        public IModel Model => internalModel;
        public IModel PrimaryModel => primaryModel;

        public IEngine PrimaryStater => this;


        private void Awake()
        {
            if (StaticModel.ContainsKey(KEY))
                throw new Exception("Engine already running");
            m_InternalEventsData = new Dictionary<string, UnityEventBase>();
            m_MainEventsData = new Dictionary<string, UnityEventBase>();
            internalEventer = new Eventer(m_InternalEventsData);
            primaryEventer = new Eventer(m_MainEventsData);
            internalModel = new DynamicModel(internalEventer);
            primaryModel = new DynamicModel(primaryEventer);
            stater = new Stater(internalModel, primaryModel);
            StaticModel.Add(KEY, (IEngine)this);
        }
    }
}
