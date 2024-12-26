namespace StateEngine.Example
{
    using StateEngine.States;

    using UnityEngine;

    public class MainState : State
    {
        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Enter()
        {
            StateEventer.Add("LocalButton", LocalButton);
            StateEventer.Add("GlobalButton", GlobalButton);
        }

        private void LocalButton()
        {
            Debug.Log("локальная кнопка вызываемая в глобальном стейтере");
        }

        private void GlobalButton()
        {
            Debug.Log("глобалььная кнопка вызываемая в глобальном стейтере");/*
            Model.Set("global", "global" + Random.Range(0, 100));
            Viewer.Push("Tesst");*/
        }

        public override void Exit()
        {
            StateEventer.Remove("LocalButton", LocalButton);
            StateEventer.Remove("GlobalButton", GlobalButton);
        }
    }
}