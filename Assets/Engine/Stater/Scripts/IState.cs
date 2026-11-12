using System;
using StateEngine.Events;

namespace StateEngine.States
{
    public interface IState : IDisposable
    {
        string Name { get; }

        bool IsInitialized { get; }

        /// <summary>
        /// Events routed to this state while it is active.
        /// </summary>
        IEventer Events { get; }

        void Initialize(StateContext context);

        void Enter();

        void Exit();
    }
}
