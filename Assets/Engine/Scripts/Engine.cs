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
        protected DynamicModel mainModel { get; set; }
        protected Eventer internalEventer { get; set; }
        protected Eventer mainEventer { get; set; }
        protected Stater stater { get; set; }
        public IEventer Eventer => internalEventer;
        public IStater Stater => stater;
        public IModel InternalModel => internalModel;
        public IModel MainModel => mainModel;

        private void Awake()
        {
            if (StaticModel.ContainsKey(KEY))
                throw new Exception("Engine already running");
            m_InternalEventsData = new Dictionary<string, UnityEventBase>();
            m_MainEventsData = new Dictionary<string, UnityEventBase>();
            internalEventer = new Eventer(m_InternalEventsData);
            mainEventer = new Eventer(m_MainEventsData);
            internalModel = new DynamicModel(internalEventer);
            mainModel = new DynamicModel(mainEventer);
            stater = new Stater(internalModel, mainModel);
            StaticModel.Add(KEY, (IEngine)this);
        }
    }
}
