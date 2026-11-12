using StateEngine.Events;
using StateEngine.Models;
using StateEngine.Views;

namespace StateEngine.States
{
    public abstract class State : IState
    {
        private Eventer m_Events;

        public virtual string Name => GetType().Name;

        public bool IsInitialized { get; private set; }

        public IEventer Events => m_Events;

        protected IStater Stater { get; private set; }

        protected IModel Model { get; private set; }

        protected IModel PrimaryModel { get; private set; }

        protected IViewer Viewer { get; private set; }

        public void Initialize(StateContext context)
        {
            if (IsInitialized)
                return;

            Stater = context.Stater;
            Model = context.Model;
            PrimaryModel = context.PrimaryModel;
            Viewer = context.Viewer;
            m_Events = new Eventer();
            IsInitialized = true;

            OnInitialize();
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public virtual void Dispose()
        {
            m_Events?.Dispose();
        }

        /// <summary>
        /// Subscribe to <see cref="Events"/> here. Subscriptions live for the whole lifetime of the state,
        /// the stater only routes events to the active one.
        /// </summary>
        protected abstract void OnInitialize();

        protected void Send(string type) => Model.Eventer.Invoke(type);

        protected void Send<T>(string type, T data) => Model.Eventer.Invoke(type, data);
    }
}
