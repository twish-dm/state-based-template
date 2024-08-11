
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace StateEngine.Events
{
    public class Eventer : IEventer
    {
        public Eventer()
        {
            m_EventsData = new Dictionary<string, UnityEventBase>();
        }
        public Eventer(Dictionary<string, UnityEventBase> eventsData) : this()
        {
            m_EventsData = eventsData;
        }
        private Dictionary<string, UnityEventBase> m_EventsData;

        public void Add(string type, UnityAction action)
        {
            if (!m_EventsData.ContainsKey(type))
                m_EventsData.Add(type, new UnityEvent());
            ((UnityEvent)m_EventsData[type]).AddListener(action);
        }

        public void Add<T>(string type, UnityAction<T> action) where T : IEventData
        {
            if (!m_EventsData.ContainsKey(type))
                m_EventsData.Add(type, new UnityEvent<T>());
            ((UnityEvent<T>)m_EventsData[type]).AddListener(action);
        }
        public void Invoke(string type)
        {
            if (!m_EventsData.ContainsKey(type)) return;
            (m_EventsData[type] as UnityEvent)?.Invoke();
        }

        public void Invoke<T>(string type, T eventData) where T : IEventData
        {
            if (!m_EventsData.ContainsKey(type)) return;
            (m_EventsData[type] as UnityEvent<T>)?.Invoke(eventData);

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
        public void Remove<T>(string type, UnityAction<T> action) where T : IEventData
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