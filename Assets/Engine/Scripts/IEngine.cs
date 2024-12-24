namespace StateEngine
{
    using StateEngine.Model;

    public interface IEngine : IStaterBehaviour
    {
        IModel PrimaryModel { get; }
    }
}
