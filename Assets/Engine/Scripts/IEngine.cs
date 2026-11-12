using StateEngine.Models;

namespace StateEngine
{
    public interface IEngine : IStateContext
    {
        /// <summary>
        /// Model shared by every state machine in the scene.
        /// </summary>
        IModel PrimaryModel { get; }
    }
}
