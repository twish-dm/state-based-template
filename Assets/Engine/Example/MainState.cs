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
            Eventer.Add("LocalButton", LocalButton);
            Eventer.Add("GlobalButton", GlobalButton);
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
            Eventer.Remove("LocalButton", LocalButton);
            Eventer.Remove("GlobalButton", GlobalButton);
        }
    }
}