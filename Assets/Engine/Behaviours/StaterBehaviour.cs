
namespace StateEngine.Behaviours
{
    using StateEngine.Events;
    using StateEngine.Model;
    using StateEngine.States;

    using UnityEngine;
    
    public class StaterBehaviour : DynamicBehaviour, IStateBehaviour
    {
        public IModel Model => model;

        public IEventer Eventer => eventer;

        virtual protected Stater stater { get; set; }
        virtual public IStater Stater => stater;
        override protected void Awake()
        {
            base.Awake();
            stater = new Stater(model);
        }
    }
}
