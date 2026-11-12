using StateEngine.Models;
using UnityEngine;

namespace StateEngine.Components.Navigation
{
    public interface INavigationStep : IPriority
    {
        Vector3 Position { get; }

        void OnStepStarted(GameObject traveller);

        void OnStepReached(GameObject traveller);
    }
}
