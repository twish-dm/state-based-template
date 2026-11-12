using StateEngine.Behaviours;
using UnityEngine;
using UnityEngine.AI;

namespace StateEngine.Components.Navigation
{
    /// <summary>
    /// Moves the agent in the direction received as a <see cref="Vector3"/> model value.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class DirectionBehaviour : BindingBehaviour
    {
        private NavMeshAgent m_Agent;

        protected override string KeySuffix => "Direction";

        protected override void OnInitialize()
        {
            m_Agent = GetComponent<NavMeshAgent>();
            Eventer.Add<Vector3>(Key, MoveInDirection);
        }

        protected override void OnRelease()
        {
            Eventer.Remove<Vector3>(Key, MoveInDirection);
        }

        private void MoveInDirection(Vector3 direction)
        {
            if (!m_Agent.isActiveAndEnabled || !m_Agent.isOnNavMesh)
                return;

            if (direction.sqrMagnitude < Mathf.Epsilon)
            {
                m_Agent.ResetPath();
                return;
            }

            Vector3 target = transform.position + direction;
            float searchRadius = direction.magnitude + m_Agent.radius;

            if (NavMesh.SamplePosition(target, out NavMeshHit hit, searchRadius, m_Agent.areaMask))
                m_Agent.SetDestination(hit.position);
        }
    }
}
