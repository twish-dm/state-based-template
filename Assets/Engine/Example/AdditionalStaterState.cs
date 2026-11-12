using StateEngine.States;
using UnityEngine;

namespace StateEngine.Example
{
    public sealed class AdditionalStaterState : State
    {
        private int m_LocalClicks;

        protected override void OnInitialize()
        {
            Events.Add(ExampleEvents.LocalButton, OnLocalButton);
            Events.Add(ExampleEvents.GlobalButton, OnGlobalButton);
        }

        private void OnLocalButton()
        {
            m_LocalClicks++;
            Model.Set(ExampleEvents.LocalText, $"Local clicks: {m_LocalClicks}");
        }

        private void OnGlobalButton()
        {
            Debug.Log($"[{Name}] {ExampleEvents.GlobalButton} handled by the local state machine");
        }
    }
}
