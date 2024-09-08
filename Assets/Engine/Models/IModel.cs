namespace StateEngine.Model
{
    using StateEngine.Events;
    using System.Collections.Generic;
    public interface IModel
    {
        IEventer Eventer { get; }
        void Refresh<T>(string key);
        void Set(string key, object value);
        void Clear();
        bool Contains(string key);
        bool Remove(string key);
        T Get<T>(string key, T def = default);
        float GetFloat(string key, float def = default);
        int GetInt(string key, int def = default);
        string GetString(string key, string def = default);
        bool GetBool(string key, bool def = default);
        DynamicList<T> CreateList<T>(string key);
        DynamicList<T> CreateList<T>(string key, params T[] values);
        DynamicList<T> GetList<T>(string key, DynamicList<T> def = default);

    }
}