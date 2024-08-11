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
            Debug.Log("Кнопка вызываемая в локальном стейтере");
        }

        private void GlobalButton()
        {
            Debug.Log("нопка вызываемая в глобальном стейтере");
            Model.Set("global", "global" + Random.Range(0, 100));
            Viewer.Push("Tesst");
        }

        public override void Exit()
        {
            Eventer.Remove("LocalButton", LocalButton);
            Eventer.Remove("GlobalButton", GlobalButton);
        }
    }
}