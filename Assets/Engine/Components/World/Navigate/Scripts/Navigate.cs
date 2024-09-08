namespace StateEngine.Components.Navigation
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;
    using UnityEngine.AI;
    using UnityEngine.Events;

    public class Navigate
    {
        public enum NavType { CalculatePath, NavMeshAgent };
        public NavType NavigationType { get; set; }
        private bool m_IsPaused;
        public bool IsPaused
        {
            get => m_IsPaused; set
            {
                if (NavigationType == NavType.NavMeshAgent)
                {
                    m_IsPaused = value;
                    m_NavMeshAgent.enabled = !m_IsPaused;
                    if (m_NavMeshAgent.enabled && m_MoveQueue != null)
                        m_Parent.StartCoroutine(m_MoveQueue);
                    else if (m_MoveQueue != null)
                    {
                        m_Parent.StopCoroutine(m_MoveQueue);
                    }
                }
                else
                {
                    m_IsPaused = value;
                }

            }
        }
        public Navigate(float pathEndThreshold = 0.5f)
        {
            m_TargetsList = new List<INavigationStep>();
            m_TargetsQueue = new Queue<INavigationStep>();
            m_PathEndThreshold = pathEndThreshold;
            IsStart = false;
            IsComplete = false;
        }
        public event UnityAction OnComplete, OnStart, OnStep;


        protected List<INavigationStep> m_TargetsList;
        protected Queue<INavigationStep> m_TargetsQueue;
        protected NavMeshAgent m_NavMeshAgent;
        protected MonoBehaviour m_Parent;
        protected IEnumerator m_MoveQueue;
        protected float m_PathEndThreshold = 0.1f;
        protected bool m_HasPath = false;
        protected Vector3 m_LastPosition;
        public bool IsLoop { get; set; }
        protected bool m_Enabled;

        virtual public bool IsComplete { get; protected set; } = false;
        virtual public bool IsStart { get; protected set; } = false;
        virtual public bool Enabled
        {
            get
            {
                return m_Enabled;
            }
            set
            {
                m_Enabled = value;
                m_NavMeshAgent.enabled = m_Enabled;
            }
        }
        virtual public void Init<T>(T target) where T : MonoBehaviour
        {
            m_NavMeshAgent = target.GetComponent<NavMeshAgent>();

            if (!m_NavMeshAgent)
                throw new NullReferenceException($"'NavMeshAgent' not found");

            m_NavMeshAgent.avoidancePriority = UnityEngine.Random.Range(0, int.MaxValue);
            m_Parent = target;
            m_LastPosition = m_Parent.transform.position;
        }

        virtual public void RemoveTarget(INavigationStep target)
        {
            m_TargetsList.Remove(target);
        }
        virtual public void ApplyTarget(INavigationStep target)
        {
            if (!m_TargetsList.Contains(target))
            {
                m_TargetsList.Add(target);
            }

            if (m_MoveQueue == null)
            {
                IsStart = true;
                IsComplete = false;
                m_HasPath = false;

                m_NavMeshAgent.enabled = true;
                m_MoveQueue = MoveQueue(0);
                Enabled = true;
                m_Parent?.StartCoroutine(m_MoveQueue);

            }
        }
        virtual public void ClearTargets()
        {
            m_TargetsQueue.Clear();
            m_TargetsList.Clear();
        }
        virtual public void Stop()
        {
            m_HasPath = false;
            IsStart = false;
            IsComplete = true;
            m_Parent?.StopCoroutine(m_MoveQueue);
            m_MoveQueue = null;
            OnComplete?.Invoke();
        }
        protected Vector3 GetNavMeshPoint(Vector3 point, float maxDistance = float.MaxValue)
        {
            return NavMesh.Raycast(point + Vector3.up, point, out NavMeshHit hit, m_NavMeshAgent.areaMask) ? hit.position : point;
        }
        virtual protected IEnumerator MoveTowards(INavigationStep step)
        {
            Vector3 target = GetNavMeshPoint(step.StepPosition);
            Vector3 forward = Vector3.zero;
            Vector3 nextStep = Vector3.zero;
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(m_NavMeshAgent.transform.position, target, m_NavMeshAgent.areaMask, path);
            int index = 0;
            while (index < path.corners.Length)
            {
                yield return new WaitForFixedUpdate();
                yield return new WaitWhile(() => IsPaused);
                nextStep = Vector3.MoveTowards(m_Parent.transform.position, path.corners[index], m_NavMeshAgent.speed * Time.fixedDeltaTime);
                forward = (path.corners[index] - m_Parent.transform.position).normalized;
                m_Parent.transform.rotation = Quaternion.RotateTowards(m_Parent.transform.rotation, Quaternion.LookRotation(forward, Vector3.up), m_NavMeshAgent.angularSpeed * Time.fixedDeltaTime);
                m_Parent.transform.position = nextStep;
                if (Equals(m_Parent.transform.position, path.corners[index]))
                    index++;
            }
        }
        virtual protected IEnumerator MoveQueue(int index)
        {
            if (NavigationType == NavType.CalculatePath)
            {
                m_NavMeshAgent.enabled = false;
                yield return null;
                m_TargetsList = m_TargetsList.OrderBy(x => x.Priority).ToList();

                IEnumerator move = null;
                m_TargetsList[index].StepStarted(m_Parent.gameObject);
                OnStart?.Invoke();
                while (index < m_TargetsList.Count)
                {
                    move = MoveTowards(m_TargetsList[index]);
                    yield return move;
                    m_TargetsList[index].StepReached(m_Parent.gameObject);
                    OnStep?.Invoke();
                    index++;
                    if (IsLoop && index >= m_TargetsList.Count)
                    {
                        m_TargetsList = m_TargetsList.OrderBy(x => x.Priority).ToList();
                        index = 0;
                    }
                }

                Stop();
                m_NavMeshAgent.enabled = true;
            }
            else
            {
                if (m_NavMeshAgent.enabled)
                    m_NavMeshAgent.ResetPath();
                m_NavMeshAgent.destination = GetNavMeshPoint(m_TargetsList[index].StepPosition);
                m_TargetsList[index].StepStarted(m_Parent.gameObject);
                OnStart?.Invoke();
                while (!IsEndOfPath())
                {
                    yield return null;
                }
                m_TargetsList[index].StepReached(m_Parent.gameObject);
                OnStep?.Invoke();
                index = IsLoop && index == m_TargetsList.Count - 1 ? 0 : index + 1;
                if (index < m_TargetsList.Count)
                {
                    yield return null;
                    m_Parent.StopCoroutine(m_MoveQueue);
                    m_MoveQueue = MoveQueue(index);
                    m_Parent.StartCoroutine(m_MoveQueue);
                    yield break;
                }
                else
                {
                    Stop();
                }
            }
        }





        protected bool IsEndOfPath()
        {
            m_HasPath |= m_NavMeshAgent.hasPath;


            if (m_HasPath && m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance + m_PathEndThreshold)
            {
                m_HasPath = false;
                m_LastPosition = m_Parent.transform.position;
                return true;
            }
            if (Vector3.Distance(m_LastPosition, m_Parent.transform.position) < m_PathEndThreshold * Time.deltaTime)
            {
                m_HasPath = false;
                m_LastPosition = m_Parent.transform.position;
                return true;
            }
            m_LastPosition = m_Parent.transform.position;
            m_HasPath = false;
            return false;
        }
    }
}