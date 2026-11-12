using System;
using System.Collections.Generic;
using StateEngine.Models;
using StateEngine.Views;

namespace StateEngine.States
{
    public sealed class Stater : IStater, IDisposable
    {
        private readonly Dictionary<string, IState> m_States = new Dictionary<string, IState>();
        private readonly IModel m_Model;
        private readonly IModel m_PrimaryModel;

        public Stater(IModel model, IModel primaryModel)
        {
            m_Model = model ?? throw new ArgumentNullException(nameof(model));
            m_PrimaryModel = primaryModel ?? throw new ArgumentNullException(nameof(primaryModel));
        }

        public IState Current { get; private set; }

        public void Add(IState state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            if (m_States.ContainsKey(state.Name))
                throw new InvalidOperationException($"State '{state.Name}' is already registered.");

            m_States.Add(state.Name, state);

            ServiceRegistry.TryGet(out IViewer viewer);
            state.Initialize(new StateContext(this, m_Model, m_PrimaryModel, viewer));
        }

        public void Remove(string stateName)
        {
            if (!m_States.TryGetValue(stateName, out IState state))
                return;

            if (ReferenceEquals(state, Current))
            {
                Current.Exit();
                Current = null;
            }

            m_States.Remove(stateName);
            state.Dispose();
        }

        public void Change(string stateName)
        {
            if (!m_States.TryGetValue(stateName, out IState next))
                throw new KeyNotFoundException($"State '{stateName}' is not registered.");

            if (ReferenceEquals(next, Current))
                return;

            Current?.Exit();
            Current = next;
            Current.Enter();
        }

        public void Invoke(string type) => Current?.Events.Invoke(type);

        public void Invoke<T>(string type, T data) => Current?.Events.Invoke(type, data);

        public void Dispose()
        {
            Current?.Exit();
            Current = null;

            foreach (IState state in m_States.Values)
                state.Dispose();

            m_States.Clear();
        }
    }
}
