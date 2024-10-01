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
            eventer.Add<DataChangeEvent>(modelField, DataChangeHandler);
        }
        private void DataChangeHandler(DataChangeEvent data)
        {
            if (TryGetComponent(out TextMeshProUGUI tmpro))
            {
                tmpro.text = data.GetData<string>();
            }
            else if (TryGetComponent(out Text text))
            {
                text.text = data.GetData<string>();
            }
        }

        public override void Dispose()
        {
            eventer.Remove<DataChangeEvent>(modelField, DataChangeHandler);
            base.Dispose();
        }
    }
}