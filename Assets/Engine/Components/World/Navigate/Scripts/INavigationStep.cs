namespace StateEngine.Components.Navigation
{
    using StateEngine.Models;

    using UnityEngine;
    public interface INavigationStep : IPriority
    {
        Vector3 StepPosition { get; }
        void StepReached(GameObject entity);
        void StepStarted(GameObject entity);
    }
}