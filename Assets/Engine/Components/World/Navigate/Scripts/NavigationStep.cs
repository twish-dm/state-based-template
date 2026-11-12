using StateEngine.Behaviours;
using StateEngine.Models;
using UnityEngine;

namespace StateEngine.Components.Navigation
{
    /// <summary>
    /// Registers its position as a waypoint in a model list consumed by <see cref="NavigateBehaviour"/>.
    /// </summary>
    public class NavigationStep : DynamicBehaviour, INavigationStep
    {
        [SerializeField, Tooltip("Lower values are visited first.")]
        private int m_Priority;

        [SerializeField, Tooltip("Model key of a DynamicList<INavigationStep>.")]
        private string m_StepsKey;

        public int Priority => m_Priority;

        public Vector3 Position => transform.position;

        protected override void OnInitialize()
        {
            if (string.IsNullOrEmpty(m_StepsKey))
            {
                Debug.LogError($"{nameof(NavigationStep)} on '{name}' has no steps key assigned.", this);
                enabled = false;
            }
        }

        protected virtual void Start()
        {
            DynamicList<INavigationStep> steps = Model.GetList<INavigationStep>(m_StepsKey)
                ?? Model.CreateList<INavigationStep>(m_StepsKey);

            if (!steps.Contains(this))
                steps.Add(this);
        }

        protected override void OnRelease()
        {
            if (!string.IsNullOrEmpty(m_StepsKey))
                Model.GetList<INavigationStep>(m_StepsKey)?.Remove(this);
        }

        public virtual void OnStepStarted(GameObject traveller)
        {
        }

        public virtual void OnStepReached(GameObject traveller)
        {
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.25f);
        }
    }
}
