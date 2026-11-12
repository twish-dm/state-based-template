using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace StateEngine.Components.Navigation
{
    /// <summary>
    /// Moves a <see cref="NavMeshAgent"/> through a list of steps ordered by priority.
    /// Runs as a coroutine on the host behaviour.
    /// </summary>
    public sealed class Navigator
    {
        private const float k_SampleDistance = 2f;
        private const float k_ArrivalDistanceSqr = 0.0001f;

        private static readonly WaitForFixedUpdate s_WaitForFixedUpdate = new WaitForFixedUpdate();

        private readonly MonoBehaviour m_Host;
        private readonly Transform m_Transform;
        private readonly NavMeshAgent m_Agent;
        private readonly NavMeshPath m_Path = new NavMeshPath();
        private readonly List<INavigationStep> m_Steps = new List<INavigationStep>();
        private readonly float m_ArrivalThreshold;

        private Coroutine m_Routine;
        private bool m_IsPaused;

        public Navigator(MonoBehaviour host, float arrivalThreshold = 0.5f)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            if (!host.TryGetComponent(out m_Agent))
                throw new MissingComponentException($"'{host.name}' requires a {nameof(NavMeshAgent)} component.");

            m_Host = host;
            m_Transform = host.transform;
            m_ArrivalThreshold = arrivalThreshold;
            m_Agent.avoidancePriority = UnityEngine.Random.Range(0, 100);
        }

        public event Action Started;
        public event Action<INavigationStep> StepReached;
        public event Action Completed;

        public NavigationMode Mode { get; set; }

        public bool IsLooping { get; set; }

        public bool IsRunning => m_Routine != null;

        public bool IsPaused
        {
            get => m_IsPaused;
            set
            {
                m_IsPaused = value;

                if (Mode == NavigationMode.NavMeshAgent && m_Agent.isActiveAndEnabled && m_Agent.isOnNavMesh)
                    m_Agent.isStopped = value;
            }
        }

        public void SetSteps(IEnumerable<INavigationStep> steps)
        {
            Stop();
            m_Steps.Clear();
            m_Steps.AddRange(steps);
            Start();
        }

        public void AddStep(INavigationStep step)
        {
            if (step == null || m_Steps.Contains(step))
                return;

            m_Steps.Add(step);
            Start();
        }

        public void RemoveStep(INavigationStep step) => m_Steps.Remove(step);

        public void Clear()
        {
            Stop();
            m_Steps.Clear();
        }

        public void Start()
        {
            if (IsRunning || m_Steps.Count == 0 || !m_Host.isActiveAndEnabled)
                return;

            m_Routine = m_Host.StartCoroutine(Run());
        }

        public void Stop()
        {
            if (m_Routine == null)
                return;

            m_Host.StopCoroutine(m_Routine);
            m_Routine = null;

            m_Agent.enabled = true;
            if (m_Agent.isActiveAndEnabled && m_Agent.isOnNavMesh)
                m_Agent.ResetPath();
        }

        private IEnumerator Run()
        {
            Started?.Invoke();
            SortSteps();

            int index = 0;
            while (index < m_Steps.Count)
            {
                INavigationStep step = m_Steps[index];
                int startFrame = Time.frameCount;

                step.OnStepStarted(m_Host.gameObject);

                IEnumerator move = Mode == NavigationMode.NavMeshAgent
                    ? MoveWithAgent(step.Position)
                    : MoveAlongPath(step.Position);

                while (move.MoveNext())
                    yield return move.Current;

                step.OnStepReached(m_Host.gameObject);
                StepReached?.Invoke(step);

                // Protects looping routes from spinning within a single frame when every target is unreachable.
                if (Time.frameCount == startFrame)
                    yield return null;

                index++;

                if (IsLooping && index >= m_Steps.Count)
                {
                    SortSteps();
                    index = 0;
                }
            }

            m_Routine = null;
            Completed?.Invoke();
        }

        private IEnumerator MoveWithAgent(Vector3 destination)
        {
            m_Agent.enabled = true;

            if (!m_Agent.isOnNavMesh || !m_Agent.SetDestination(SampleNavMesh(destination)))
                yield break;

            m_Agent.isStopped = m_IsPaused;

            while (m_Agent.pathPending || m_Agent.remainingDistance > m_Agent.stoppingDistance + m_ArrivalThreshold)
            {
                if (!m_Agent.pathPending && !m_Agent.hasPath)
                    yield break;

                yield return null;
            }
        }

        private IEnumerator MoveAlongPath(Vector3 destination)
        {
            if (!NavMesh.CalculatePath(m_Transform.position, SampleNavMesh(destination), m_Agent.areaMask, m_Path))
                yield break;

            m_Agent.enabled = false;

            foreach (Vector3 corner in m_Path.corners)
            {
                while ((corner - m_Transform.position).sqrMagnitude > k_ArrivalDistanceSqr)
                {
                    yield return s_WaitForFixedUpdate;

                    if (m_IsPaused)
                        continue;

                    float deltaTime = Time.fixedDeltaTime;
                    Vector3 heading = corner - m_Transform.position;
                    heading.y = 0f;

                    if (heading.sqrMagnitude > k_ArrivalDistanceSqr)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(heading, Vector3.up);
                        m_Transform.rotation = Quaternion.RotateTowards(m_Transform.rotation, targetRotation, m_Agent.angularSpeed * deltaTime);
                    }

                    m_Transform.position = Vector3.MoveTowards(m_Transform.position, corner, m_Agent.speed * deltaTime);
                }
            }

            m_Agent.enabled = true;
        }

        private Vector3 SampleNavMesh(Vector3 point)
        {
            return NavMesh.SamplePosition(point, out NavMeshHit hit, k_SampleDistance, m_Agent.areaMask) ? hit.position : point;
        }

        private void SortSteps() => m_Steps.Sort((a, b) => a.Priority.CompareTo(b.Priority));
    }
}
