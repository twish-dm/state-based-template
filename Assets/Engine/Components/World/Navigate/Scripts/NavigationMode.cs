namespace StateEngine.Components.Navigation
{
    public enum NavigationMode
    {
        /// <summary>
        /// Moves the transform along a precalculated NavMesh path. The agent is disabled while moving.
        /// </summary>
        CalculatePath,

        /// <summary>
        /// Delegates movement to <see cref="UnityEngine.AI.NavMeshAgent"/>.
        /// </summary>
        NavMeshAgent
    }
}
