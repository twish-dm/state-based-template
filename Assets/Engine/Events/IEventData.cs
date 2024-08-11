namespace StateEngine.Events
{
    public interface IEventData
    {
        int Type { get; }
        object RawData { get; }
        T GetData<T>();
    }
}