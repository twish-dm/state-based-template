namespace StateEngine.Events
{
    public class EventBase : IEventData
    {
        public EventBase(int type, object data)
        {
            Type = type;
            RawData = data;
        }
        virtual public int Type { get; }

        virtual public object RawData { get; }

        virtual public T GetData<T>()
        {
            return (T)RawData;
        }
    }
}