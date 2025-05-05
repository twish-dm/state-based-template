
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace StateEngine.Events
{
    public class Eventer : IEventer
    {
        static private int m_Id;
        public Eventer()
        {
            m_EventsData = new Dictionary<string, UnityEventBase>();
            m_Id++;
        }
        public Eventer(Dictionary<string, UnityEventBase> eventsData) : this()
        {
            m_EventsData = eventsData;
        }
        private Dictionary<string, UnityEventBase> m_EventsData;

        public int id => m_Id;

        public void Add(string type, UnityAction action)
        {
            if (!m_EventsData.ContainsKey(type))
                m_EventsData.Add(type, new UnityEvent());
            ((UnityEvent)m_EventsData[type]).AddListener(action);

            Debug.Log($"({id})[{type}] added;");
        }

        public void Add<T>(string type, UnityAction<T> action)
        {
            if (!m_EventsData.ContainsKey(type))
                m_EventsData.Add(type, new UnityEvent<T>());
            ((UnityEvent<T>)m_EventsData[type]).AddListener(action);

            Debug.Log($"({id})[{type}({typeof(T).Name})] added;"); 
        }
        public bool Contains(string key)
        {
            return m_EventsData.ContainsKey(key);
        }
        public void Add(string type, UnityAction<object> action)
        {
            if (!m_EventsData.ContainsKey(type))
                m_EventsData.Add(type, new UnityEvent<object>());
            ((UnityEvent<object>)m_EventsData[type]).AddListener(action);

            Debug.Log($"({id})[{type}({typeof(object).Name})] added;");
        }

        public void Invoke(string type)
        {
            Debug.Log($"({id})[{type}];");
            if (!m_EventsData.ContainsKey(type)) return;
            (m_EventsData[type] as UnityEvent)?.Invoke();

            Debug.Log($"({id})[{type}]; success");
        }

        public void Invoke<T>(string type, T eventData)
        {
            Debug.Log($"({id})[{type}].Invoke<T>({eventData}); {m_EventsData.ContainsKey(type)}");
            if (!m_EventsData.ContainsKey(type)) return;
            (m_EventsData[type] as UnityEvent<T>)?.Invoke(eventData);

            Debug.Log($"({id})[{type}].Invoke<T>({eventData}) success;");

        }

        public void RemoveAll(string type)
        {
            if (m_EventsData.ContainsKey(type) && m_EventsData[type] != null)
                m_EventsData[type].RemoveAllListeners();
            else
                Debug.LogWarning($"CAN'T REMOVE: [{type}=>{m_EventsData[type].GetType()}]");
        }

        public void Remove(string type, UnityAction action)
        {
            if (m_EventsData.ContainsKey(type) && m_EventsData[type] is UnityEvent)
                (m_EventsData[type] as UnityEvent).RemoveListener(action);
            else
                Debug.LogWarning($"CAN'T REMOVE: [{type}=>{m_EventsData[type].GetType()}]");
        }
        public void Remove<T>(string type, UnityAction<T> action)
        {
            if (m_EventsData.ContainsKey(type) && m_EventsData[type] is UnityEvent<T>)
                (m_EventsData[type] as UnityEvent<T>).RemoveListener(action);
            else
                Debug.LogWarning($"CAN'T REMOVE: [{type}=>{m_EventsData[type].GetType()}]");
        }

        public void Dispose()
        {
            foreach (string key in m_EventsData.Keys)
            {
                RemoveAll(key);
                m_EventsData[key] = null;
            }
            m_EventsData.Clear();
            m_EventsData = null;
        }
    }
}