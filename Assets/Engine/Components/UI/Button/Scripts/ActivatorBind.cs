namespace StateEngine.Components
{
    using StateEngine.Behaviours;

    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class ActivatorBind : DynamicBehaviour
    {
        [SerializeField] protected string modelField;
        [SerializeField] protected bool defaultValue;
        [SerializeField] protected bool isInversed;
        protected Button button;
        private void OnValidate()
        {
            modelField = $"{(string.IsNullOrEmpty(modelField) ? name + "Activator" : modelField)}";
        }
        override public void Initialize()
        {
            modelField = $"{(string.IsNullOrEmpty(modelField) ? name + "Activator" : modelField)}";
            eventer.Add<bool>(modelField, ActivateHandler);
        }
        private void Start()
        {
            Debug.Log($"{name} eventer=>{eventer.id}");
            Debug.Log($"{modelField}=>{internalModel.GetBool(modelField, defaultValue)} && && {eventer == internalModel.Eventer}");
            ActivateHandler(internalModel.GetBool(modelField, defaultValue));
        }
        private void ActivateHandler(bool value)
        {
            Debug.Log($"ActivateHandler ({internalModel.id}){modelField}=>{internalModel.GetBool(modelField, defaultValue)} && && {eventer == internalModel.Eventer}");
            if (isInversed)
                value = !value;
            gameObject.SetActive(value);
        }

        public override void Dispose()
        {
            //eventer.Remove<bool>(modelField, ActivateHandler);
            base.Dispose();
        }
    }
}