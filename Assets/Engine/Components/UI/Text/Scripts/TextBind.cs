namespace StateEngine.Components
{
    using StateEngine.Behaviours;
    using StateEngine.Events;

    using TMPro;

    using UnityEngine;
    using UnityEngine.UI;

    public class TextBind : DynamicBehaviour
    {
        [SerializeField] protected string modelField;
        override public void Initialize()
        {
            modelField = $"{(string.IsNullOrEmpty(modelField) ? name+ "Text" : modelField)}";

            Debug.Log(eventer);
            eventer.Add<string>(modelField, DataChangeHandler);
        }
        private void Start()
        {
            DataChangeHandler(internalModel.GetString(modelField, ""));
        }
        private void DataChangeHandler(string data)
        {
            if (TryGetComponent(out TextMeshProUGUI tmpro))
            {
                tmpro.text = data.ToString();
            }
            else if (TryGetComponent(out Text text))
            {
                text.text = data.ToString();
            }
        }

        public override void Dispose()
        {
            eventer.Remove<string>(modelField, DataChangeHandler);
            base.Dispose();
        }
    }
}