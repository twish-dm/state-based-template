namespace StateEngine.Example
{
    using StateEngine.States;

    using UnityEngine;

    public class AdditionalStaterState : State
    {
        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Enter()
        {
            Eventer.Add("LocalButton", LocalButton);
            Eventer.Add("GlobalButton1", GlobalButton);
        }

        private void LocalButton()
        {
            Debug.Log("локальная кнопка вызываемая в локальная стейтере");
        }

        private void GlobalButton()
        {
            Debug.Log("глобалььная кнопка вызываемая в локальная стейтере");
        }

        public override void Exit()
        {
            Eventer.Remove("LocalButton", LocalButton);
            Eventer.Remove("GlobalButton1", GlobalButton);
        }
    }
}