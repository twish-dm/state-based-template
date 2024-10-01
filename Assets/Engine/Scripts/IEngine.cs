namespace StateEngine
{
    using StateEngine.Model;

    public interface IEngine : IStateBehaviour
    {
        IModel MainModel { get; }
    }
}
