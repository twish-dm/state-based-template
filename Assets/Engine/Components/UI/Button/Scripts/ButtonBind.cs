namespace StateEngine.Components
{
    using StateEngine.Behaviours;

    using System;

    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class ButtonBind : DynamicBehaviour
    {
        [SerializeField] protected string modelField;
        protected Button button;
        override public void Initialize()
        {
            modelField = $"{(string.IsNullOrEmpty(modelField) ? name+ "Button" : modelField)}";
            button = GetComponent<Button>();
            button.onClick.AddListener(ClickHandler);
        }

        private void ClickHandler()
        {
            Send(modelField);
        }

        public override void Dispose()
        {
            button.onClick.RemoveListener(ClickHandler);
            base.Dispose();
        }
    }
}