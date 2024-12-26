namespace StateEngine
{
    using StateEngine.Events;
    using StateEngine.Model;
    using StateEngine.States;

    public interface IStaterBehaviour
    {
        IModel Model { get; }
        IEventer Eventer { get; }
        IStater Stater { get; }
        IEngine PrimaryStater { get; }
        IStaterBehaviour ParentStaterBehaviour { get; }

    }
}