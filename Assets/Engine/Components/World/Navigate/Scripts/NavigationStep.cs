namespace StateEngine.Components.Navigation
{
    using StateEngine.Behaviours;

    using UnityEngine;

    public class NavigationStep : DynamicBehaviour, INavigationStep
    {

        [field: SerializeField] public int Priority { get; protected set; }
        [Header("Ссылка на DynamicList<INavigationStep>")]
        [SerializeField] protected string stepsField;

        public Vector3 StepPosition => transform.position;


        private void Start()
        {
            if (string.IsNullOrEmpty(stepsField))
            {
                throw new System.Exception("Добавьте ссылку на DynamicList<INavigationStep>");
            }
            if (!model.Contains(stepsField))
                model.CreateList<INavigationStep>(stepsField);
            if (!model.GetList<INavigationStep>(stepsField).Contains(this))
                model.GetList<INavigationStep>(stepsField).Add(this);
        }
        virtual protected void OnEnable()
        {

        }
        virtual protected void OnDisable()
        {

        }
        public override void Dispose()
        {
            if (model.Contains(stepsField) && model.GetList<INavigationStep>(stepsField).Contains(this))
                model.GetList<INavigationStep>(stepsField).Remove(this);
            base.Dispose();
        }

        public void StepReached(GameObject entity)
        {
            Debug.Log("STEP REACHED");
        }

        public void StepStarted(GameObject entity)
        {
            Debug.Log("STEP STARTED");
        }
    }
}