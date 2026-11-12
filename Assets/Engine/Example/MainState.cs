using StateEngine.States;
using UnityEngine;

namespace StateEngine.Example
{
    public sealed class MainState : State
    {
        private int m_GlobalClicks;

        protected override void OnInitialize()
        {
            Events.Add(ExampleEvents.LocalButton, OnLocalButton);
            Events.Add(ExampleEvents.GlobalButton, OnGlobalButton);
        }

        private void OnLocalButton()
        {
            Debug.Log($"[{Name}] {ExampleEvents.LocalButton}");
        }

        private void OnGlobalButton()
        {
            m_GlobalClicks++;
            Model.Set(ExampleEvents.GlobalText, $"Global clicks: {m_GlobalClicks}");
        }
    }
}
