namespace StateEngine.Events
{
    public interface IEventData
    {
        int Type { get; }
        T GetData<T>();
    }
}