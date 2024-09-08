namespace StateEngine.Components.Navigation
{

    using StateEngine.Behaviours;
    using StateEngine.Events;
    using StateEngine.Model;

    using System;
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.AI;

    [RequireComponent(typeof(NavMeshAgent))]
    public class NavigateBehaviour : DynamicBehaviour
    {
        [SerializeField] private bool m_IsLoop;
        [SerializeField] private Navigate.NavType m_NavigationType;
        [Header("—сылка на DynamicList<INavigationStep>")]
        [SerializeField] protected string stepsField;
        protected Navigate directionNavigate;
        [SerializeField] private bool m_IsPaused;
        public bool IsPaused
        {
            get => directionNavigate != null ? directionNavigate.IsPaused : false;
            set { if (directionNavigate != null) directionNavigate.IsPaused = value; }
        }
        public bool IsLoop
        {
            get => directionNavigate != null && directionNavigate.IsLoop;
            set { if (directionNavigate != null) directionNavigate.IsLoop = value; }
        }
        private Navigate.NavType navigationType;
        public Navigate.NavType NavigationType
        {
            get => directionNavigate.NavigationType;
            set { if (directionNavigate != null) directionNavigate.NavigationType = value; }
        }

        private void OnValidate()
        {
            if (IsLoop != m_IsLoop)
                IsLoop = m_IsLoop;
            if (IsPaused != m_IsPaused)
                IsPaused = m_IsPaused;
        }
        public override void Initialize()
        {
            Reset();
            directionNavigate = new Navigate();
            IsLoop = m_IsLoop;
            IsPaused = m_IsPaused;
            NavigationType = m_NavigationType;
            directionNavigate.Init(this);
            eventer.Add<ListEvent<INavigationStep>>(stepsField, StepsHandler);
        }
        virtual protected void Start()
        {
            ApplySteps(model.GetList<INavigationStep>(stepsField));
        }
        private void Reset()
        {

            stepsField = $"{(string.IsNullOrEmpty(stepsField) ? name + "Steps" : stepsField)}";
        }


        protected void ApplySteps(DynamicList<INavigationStep> steps, INavigationStep modifiedItem = null)
        {
            if (modifiedItem != null)
                directionNavigate.ApplyTarget(modifiedItem);
            else if (steps != null && steps.Count > 0)
            {
                directionNavigate.ClearTargets();
                for (int i = 0; i < steps.Count; i++)
                    directionNavigate.ApplyTarget(steps[i]);
            }
            else
            {
                directionNavigate.Stop();
                directionNavigate.ClearTargets();
            }
        }
        virtual protected void StepsHandler(ListEvent<INavigationStep> stepsEvent)
        {
            ApplySteps(stepsEvent.List, stepsEvent.ModifiedItem);
        }
        public override void Dispose()
        {
            eventer.Remove<ListEvent<INavigationStep>>(stepsField, StepsHandler);
           
            base.Dispose();
        }
    }
    public class DirectionBehaviour : DynamicBehaviour
    {

        [Header("—сылка на Vector3")]
        [SerializeField] protected string directionField;
        protected DirectionNavigate directionNavigate;
        public override void Initialize()
        {
            directionNavigate = new DirectionNavigate();
            directionNavigate.IsLoop = false;
            directionNavigate.IsPaused = false;
            directionNavigate.NavigationType = Navigate.NavType.NavMeshAgent;
            directionNavigate.Init(this);
            eventer.Add<DataChangeEvent>(directionField, DirectionHandler);
            directionField = $"{(string.IsNullOrEmpty(directionField) ? name + "Direction" : directionField)}";
            
        }
        public override void Dispose()
        {
            eventer.Remove<DataChangeEvent>(directionField, DirectionHandler);
            base.Dispose();
        }
        virtual protected void DirectionHandler(DataChangeEvent directionEvent)
        {
            directionNavigate.Move(directionEvent.GetData<Vector3>());
        }
    }
}
