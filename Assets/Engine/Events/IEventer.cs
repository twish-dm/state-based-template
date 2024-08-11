namespace StateEngine.Events
{
    using System;
    using UnityEngine.Events;
    public interface IEventer : IDisposable
    {
        void Add(string type, UnityAction action);
        void Add<T>(string type, UnityAction<T> action) where T : IEventData;
        void Invoke(string type);
        void Invoke<T>(string type, T eventData) where T : IEventData;
        void RemoveAll(string type);
        void Remove(string type, UnityAction action);
        void Remove<T>(string type, UnityAction<T> action) where T : IEventData;
    }
}