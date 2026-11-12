using StateEngine.Models;

namespace StateEngine.Events
{
    public enum ListChangeType
    {
        Added,
        Removed,
        Replaced,
        Cleared
    }

    public readonly struct ListEvent<T>
    {
        public ListEvent(ListChangeType type, DynamicList<T> list, T item = default)
        {
            Type = type;
            List = list;
            Item = item;
        }

        public ListChangeType Type { get; }

        public DynamicList<T> List { get; }

        public T Item { get; }
    }
}
