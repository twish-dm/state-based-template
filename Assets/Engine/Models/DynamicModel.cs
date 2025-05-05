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

    using UnityEngine;

    public class DynamicModel : IModel
    {
        static private int m_Id = 0;
        protected Dictionary<string, object> data;
        public IEventer Eventer { get; }
        public ICollection<string> Keys => data.Keys;
        public ICollection<object> Values => data.Values;
        public int Count => data.Count;

        public int id { get; }

        public DynamicModel(IEventer eventer)
        {
            id = m_Id;
            Eventer = eventer;
            data = new Dictionary<string, object>();
            m_Id++;
        }


        virtual public object this[string key]
        {
            get => data[key];
            set
            {
                bool contains = Contains(key);
                data[key] = value;
                Eventer.Invoke(key, value);
              // Debug.Log($"({id})[{key}]=>{value}");
            }
        }
        public void Set(string key, object value)
        {
            if (!data.ContainsKey(key))
            {
                data.Add(key, value);
            }
            else
            {
                data[key] = value;
            }


            Eventer.Invoke(key, value);
           // Debug.Log($"Set<object> e=>{Eventer.id}({id})[{key}]=>{value}");
        }
        public void Set<T>(string key, T value)
        {
            if (!data.ContainsKey(key))
            {
                data.Add(key, value);
            }
            else
            {
                data[key] = value;
            }


            Eventer.Invoke(key, value);
            //Debug.Log($"Set<T> e=>{Eventer.id}({id})[{key}]=>{value}");
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
        public DynamicList<T> CreateList<T>(string key, List<T> values)
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
                Eventer.Invoke<object>(key, null);
            data.Clear();
        }

        public void Refresh<T>(string key)
        {
            Eventer.Invoke<T>(key, Get<T>(key));
        }


        public bool Contains(string key)
        {
            return data.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            Eventer.Invoke(key, data[key]);
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
}
