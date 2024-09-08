namespace StateEngine.Events
{
    using System.Collections.Generic;
    public class EventBase : IEventData
    {
        public EventBase() { }
        public EventBase(int type, object data):this()
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