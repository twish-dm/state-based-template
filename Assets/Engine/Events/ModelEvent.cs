namespace StateEngine.Events
{
    public class ModelEvent : EventBase
    {
        public ModelEvent(int type, object data) : base(type, data) { }
        public const int TYPE_DATA_ADD = 0;
        public const int TYPE_DATA_REMOVE = 1;
        public const int TYPE_DATA_CHANGE = 2;
        public const int TYPE_DATA_REFRESH = 100;
    }
}