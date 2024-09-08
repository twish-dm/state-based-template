namespace StateEngine.Events
{
    public class DataChangeEvent : EventBase
    {
        public DataChangeEvent(int type, object data) : base(type, data) { }
        public const int TYPE_DATA_ADD = 0;
        public const int TYPE_DATA_REMOVE = 1;
        public const int TYPE_DATA_CHANGE = 2;
        public const int TYPE_DATA_REFRESH = 100;
    }
    public class ComponentActionEvent : EventBase
    {
        public ComponentActionEvent(int type, object data) : base(type, data) { }
        public const int TYPE_CLICK = 0;
    }
}