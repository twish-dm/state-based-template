
namespace StateEngine.Behaviours
{
    using StateEngine.Events;
    using StateEngine.Model;
    using StateEngine.States;

    using UnityEngine;
    
    public class StaterBehaviour : DynamicBehaviour, IStateBehaviour
    {
        public IModel InternalModel => internalModel;
        
        public IEventer Eventer => eventer;

        virtual protected Stater stater { get; set; }
        virtual public IStater Stater => stater;

        protected IModel MainModel => StaticModel.Get<IEngine>(Engine.KEY).MainModel;

        override protected void Awake()
        {
            base.Awake();
            stater = new Stater(InternalModel, MainModel);
        }
    }
}
