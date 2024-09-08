namespace StateEngine.Model
{
    using StateEngine.Events;

    using System.Collections.Generic;

    public class DynamicList<T> : List<T>, ICollection<T>
    {
        public DynamicList(string field, IEventer eventer) : base()
        {
            this.field = field;
            this.eventer = eventer;
        }
        public DynamicList(string field, IEventer eventer, ICollection<T> list) : base(list)
        {
            this.field = field;
            this.eventer = eventer;
        }
        protected IEventer eventer;
        protected string field;

        new virtual public T this[int key]
        {
            get => base[key];
            set
            {
                base[key] = value;
                eventer.Invoke<ListEvent<T>>(field, new(ListEvent.TYPE_DATA_ITEM_CHANGE, this, base[key]));
            }
        }

        new virtual public void Add(T item)
        {
            base.Add(item);
            eventer.Invoke<ListEvent<T>>(field, new(ListEvent.TYPE_DATA_ITEM_NEW, this, item));
        }

        new virtual public void Clear()
        {
            base.Clear();
            eventer.Invoke<ListEvent<T>>(field, new(ListEvent.TYPE_DATA_CLEAR, this));
        }
        new virtual public bool Remove(T item)
        {
            bool result = base.Remove(item);
            eventer.Invoke<ListEvent<T>>(field, new(ListEvent.TYPE_DATA_ITEM_REMOVE, this, item));
            return result;
        }
    }
    public class ListEvent : DataChangeEvent
    {
        public const int TYPE_DATA_ITEM_CHANGE = 3;
        public const int TYPE_DATA_ITEM_NEW = 4;
        public const int TYPE_DATA_ITEM_REMOVE = 5;
        public const int TYPE_DATA_CLEAR = 5;
        public ListEvent(int type, object data) : base(type, data) { }
    }
    public class ListEvent<T> : ListEvent
    {

        public ListEvent(int type, ICollection<T> list, T modifiedItem) : base(type, list)
        {
            ModifiedItem = modifiedItem;
        }
        public ListEvent(int type, ICollection<T> list) : base(type, list)
        {

        }

        public DynamicList<T> List => GetData<DynamicList<T>>();
        public T ModifiedItem { get; }
    }
}
