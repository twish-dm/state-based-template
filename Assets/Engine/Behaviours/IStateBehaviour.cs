namespace StateEngine
{
    using StateEngine.Events;
    using StateEngine.Model;
    using StateEngine.States;

    public interface IStateBehaviour
    {
        IModel InternalModel { get; }
        IEventer Eventer { get; }
        IStater Stater { get; }
    }
}