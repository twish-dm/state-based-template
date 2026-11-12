using StateEngine.Events;
using StateEngine.Models;
using StateEngine.States;
using UnityEngine;

namespace StateEngine
{
    [DefaultExecutionOrder(-1200)]
    public class Engine : MonoBehaviour, IEngine
    {
        private DynamicModel m_Model;
        private DynamicModel m_PrimaryModel;
        private Stater m_Stater;

        public IModel Model => m_Model;

        public IEventer Eventer => m_Model.Eventer;

        public IStater Stater => m_Stater;

        public IModel PrimaryModel => m_PrimaryModel;

        protected virtual void Awake()
        {
            ServiceRegistry.Register<IEngine>(this);

            m_Model = new DynamicModel(new Eventer());
            m_PrimaryModel = new DynamicModel(new Eventer());
            m_Stater = new Stater(m_Model, m_PrimaryModel);
        }

        protected virtual void OnDestroy()
        {
            ServiceRegistry.Unregister<IEngine>(this);

            m_Stater?.Dispose();
            m_Model?.Eventer.Dispose();
            m_PrimaryModel?.Eventer.Dispose();
        }
    }
}
