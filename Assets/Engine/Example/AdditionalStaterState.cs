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
            Eventer.Add("GlobalButton", GlobalButton);
        }

        private void LocalButton()
        {
            Debug.Log("Кнопка вызываемая в локальном стейтере");
            Model.Set("local", "local"+Random.Range(0, 100));
        }

        private void GlobalButton()
        {
            Debug.Log("нопка вызываемая в глобальном стейтере");
        }

        public override void Exit()
        {
            Eventer.Remove("LocalButton", LocalButton);
            Eventer.Remove("GlobalButton", GlobalButton);
        }
    }
}