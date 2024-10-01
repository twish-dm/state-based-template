namespace StateEngine.States
{
    using StateEngine.Model;
    using StateEngine.Events;
    using System;
    using System.Collections.Generic;
    using UnityEngine.Events;
    using StateEngine.Views;

    public class Stater : IStater
    {
        protected IModel internalModel, mainModel;
        public Stater(IModel internalModel, IModel mainModel)
        {
            this.internalModel = internalModel;
            this.mainModel = mainModel;
        }
        private Dictionary<string, int> m_StateIndexMap = new Dictionary<string, int>();
        private List<IState> m_States = new List<IState>();
        virtual public IState Current { get; protected set; }

        virtual public void Add(IState state)
        {
            if (m_StateIndexMap.ContainsKey(state.Name))
                throw new Exception($"State '{state.Name}({state.GetType()})' already exist");
            state.Stater = this;
            state.InternalModel = internalModel;
            state.Viewer = StaticModel.Get<IViewer>(Viewer.KEY);
            m_StateIndexMap.Add(state.Name, m_States.Count);
            m_States.Add(state);
            state.Initialize();
        }
        virtual public void Remove(IState state)
        {
            if (!m_StateIndexMap.ContainsKey(state.Name))
                throw new Exception($"State '{state.Name}({state.GetType()})' not found");
            m_StateIndexMap.Remove(state.Name);
            m_States.Remove(state);
        }
        virtual public void Remove(string stateName)
        {
            Remove(m_States[m_StateIndexMap[stateName]]);
        }
        virtual public void Start(string stateName)
        {
            Change(stateName);
        }
        virtual public void Change(string stateName)
        {
            if (Current != null)
                Current.Exit();

            if (!m_States[m_StateIndexMap[stateName]].IsInitialized)
                m_States[m_StateIndexMap[stateName]].Initialize();

            Current = m_States[m_StateIndexMap[stateName]];
            Current.Enter();
        }

        virtual public void Invoke(string type)
        {
            Current?.Eventer.Invoke(type);
        }

        virtual public void Invoke<T>(string type, T eventData) where T : IEventData
        {
            Current?.Eventer.Invoke(type, eventData);
        }
    }
}