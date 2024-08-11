namespace StateEngine.Model
{
    using System.Collections.Generic;
    static public class StaticModel
    {
        static StaticModel()
        {
            data = new Dictionary<string, object>();
        }
        static private Dictionary<string, object> data;
        static public ICollection<string> Keys => data.Keys;
        static public ICollection<object> Values => data.Values;
        static public int Count => data.Count;

        
        static public void Add(string key, object value)
        {
            data.Add(key, value);
        }
        /// <summary>
        /// Получить данные из статической модели
        /// </summary>
        /// <typeparam name="T">Тип получаемых данных</typeparam>
        /// <param name="key">Ключ (имя поля)</param>
        /// <returns></returns>
        static public T Get<T>(this string key)
        {
            if(data.ContainsKey(key))
                return (T)data[key];
            return default;
        }
        static public bool ContainsKey(string key)
        {
            return data.ContainsKey(key);
        }

        static public bool Remove(string key)
        {
            return data.Remove(key);
        }
    }
}
