using StateEngine.Behaviours;
using StateEngine.Events;
using StateEngine.Models;
using UnityEngine;
using UnityEngine.AI;

namespace StateEngine.Components.Navigation
{
    /// <summary>
    /// Follows the <see cref="INavigationStep"/>s stored in a <see cref="DynamicList{T}"/> of the bound model.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavigateBehaviour : DynamicBehaviour
    {
        [SerializeField] private NavigationMode m_NavigationType;
        [SerializeField] private bool m_IsLoop;
        [SerializeField] private bool m_IsPaused;

        [SerializeField, Tooltip("Model key of a DynamicList<INavigationStep>.")]
        private string m_StepsKey;

        private Navigator m_Navigator;

        public bool IsPaused
        {
            get => m_IsPaused;
            set
            {
                m_IsPaused = value;
                if (m_Navigator != null)
                    m_Navigator.IsPaused = value;
            }
        }

        public bool IsLooping
        {
            get => m_IsLoop;
            set
            {
                m_IsLoop = value;
                if (m_Navigator != null)
                    m_Navigator.IsLooping = value;
            }
        }

        protected Navigator Navigator => m_Navigator;

        private string StepsKey => string.IsNullOrEmpty(m_StepsKey) ? name + "Steps" : m_StepsKey;

        protected override void OnInitialize()
        {
            m_Navigator = new Navigator(this)
            {
                Mode = m_NavigationType,
                IsLooping = m_IsLoop,
                IsPaused = m_IsPaused
            };

            Eventer.Add<ListEvent<INavigationStep>>(StepsKey, OnStepsChanged);
        }

        protected virtual void Start()
        {
            DynamicList<INavigationStep> steps = Model.GetList<INavigationStep>(StepsKey);
            if (steps != null)
                m_Navigator.SetSteps(steps);
        }

        protected override void OnRelease()
        {
            Eventer.Remove<ListEvent<INavigationStep>>(StepsKey, OnStepsChanged);
            m_Navigator.Stop();
        }

        private void OnValidate()
        {
            IsLooping = m_IsLoop;
            IsPaused = m_IsPaused;
        }

        protected virtual void OnStepsChanged(ListEvent<INavigationStep> change)
        {
            switch (change.Type)
            {
                case ListChangeType.Added:
                    m_Navigator.AddStep(change.Item);
                    break;
                case ListChangeType.Removed:
                    m_Navigator.RemoveStep(change.Item);
                    break;
                default:
                    m_Navigator.SetSteps(change.List);
                    break;
            }
        }
    }
}
