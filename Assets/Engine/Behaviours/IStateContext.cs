using StateEngine.Events;
using StateEngine.Models;
using StateEngine.States;

namespace StateEngine
{
    /// <summary>
    /// Scope that child behaviours bind to: a model with its event bus and a state machine.
    /// </summary>
    public interface IStateContext
    {
        IModel Model { get; }

        IEventer Eventer { get; }

        IStater Stater { get; }
    }
}
