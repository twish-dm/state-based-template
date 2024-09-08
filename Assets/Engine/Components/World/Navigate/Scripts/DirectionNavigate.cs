namespace StateEngine.Components.Navigation
{

    using UnityEngine;
    using UnityEngine.AI;
    public class DirectionNavigate : Navigate
    {
        public DirectionNavigate(float pathEndThreshold = 0.1f) : base(pathEndThreshold)
        {

        }
        public void Move(Vector3 direction)
        {
            if (NavMesh.SamplePosition(m_Parent.transform.position + direction, out NavMeshHit hit, float.MaxValue, NavMesh.AllAreas))
            {
                //m_NavMeshAgent.SetDestination(hit.position);
            }
            Debug.Log(direction);
        }

    }
}