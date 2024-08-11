

namespace StateEngine.Model
{
    using StateEngine.Events;

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    public class DynamicModel : IModel
    {
        protected Dictionary<string, object> data;
        public IEventer Eventer { get; }
        public ICollection<string> Keys => data.Keys;
        public ICollection<object> Values => data.Values;
        public int Count => data.Count;


        public DynamicModel(IEventer eventer)
        {
            Eventer = eventer;
            data = new Dictionary<string, object>();
        }


        virtual public object this[string key]
        {
            get => data[key];
            set
            {
                bool contains = Contains(key);
                data[key] = value;
                Eventer.Invoke<DataChangeEvent>(key, new(contains ? DataChangeEvent.TYPE_DATA_CHANGE : DataChangeEvent.TYPE_DATA_ADD, value));
            }
        }
        public void Set(string key, object value)
        {
            if (data.ContainsKey(key))
                data[key] = value;
            else
                data.Add(key, value);
            Eventer.Invoke<DataChangeEvent>(key, new(DataChangeEvent.TYPE_DATA_ADD, value));
        }
        public DynamicList<T> CreateList<T>(string key)
        {
            DynamicList<T> list = new DynamicList<T>(key, Eventer);
            Set(key, list);
            return list;
        }
        public DynamicList<T> CreateList<T>(string key, params T[] values)
        {
            DynamicList<T> list = new DynamicList<T>(key, Eventer, values);
            Set(key, list);
            return list;
        }
        public void Add(KeyValuePair<string, object> item)
        {
            Set(item.Key, item.Value);
        }

        public void Clear()
        {
            foreach (string key in data.Keys)
                Eventer.Invoke<DataChangeEvent>(key, new(DataChangeEvent.TYPE_DATA_REMOVE, data[key]));
            data.Clear();
        }


        public bool Contains(string key)
        {
            return data.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            Eventer.Invoke<DataChangeEvent>(key, new(DataChangeEvent.TYPE_DATA_REMOVE, data[key]));
            bool result = data.Remove(key);
            return result;
        }

        public T Get<T>(string key, T def = default)
        {
            return Contains(key) ? (T)data[key] : def;
        }
        public float GetFloat(string key, float def = default)
        {
            return Contains(key) ? (float)data[key] : def;
        }
        public int GetInt(string key, int def = default)
        {
            return Contains(key) ? (int)data[key] : def;
        }
        public string GetString(string key, string def = default)
        {
            return Contains(key) ? (string)data[key] : def;
        }
        public DynamicList<T> GetList<T>(string key, DynamicList<T> def = default)
        {
            return Contains(key) ? (DynamicList<T>)data[key] : def;
        }

        public bool GetBool(string key, bool def = false)
        {
            return Contains(key) ? (bool)data[key] : def;
        }
    }

    public class DynamicList<T> : List<T>, ICollection<T>
    {
        public DynamicList(string field, IEventer eventer):base()
        {
            this.field = field;
            this.eventer = eventer;
        }
        public DynamicList(string field, IEventer eventer, ICollection<T> list) :base(list)
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
                eventer.Invoke<DataChangeEvent>(field, new(DataChangeEvent.TYPE_DATA_CHANGE, this));
            }
        }

        new virtual public void Add(T item)
        {
            base.Add(item);
            eventer.Invoke<DataChangeEvent>(field, new(DataChangeEvent.TYPE_DATA_ADD, this));
        }

        new virtual public void Clear()
        {
            base.Clear();
            eventer.Invoke<DataChangeEvent>(field, new(DataChangeEvent.TYPE_DATA_REMOVE, this));
        }
        new virtual public bool Remove(T item)
        {
            bool result = base.Remove(item);
            return result;
        }

    }
}
