namespace StateEngine.Components
{
    using StateEngine.Behaviours;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class Spawner<T> : DynamicBehaviour where T : MonoBehaviour, IDisposable
    {
        [SerializeField] protected T prefab;
        [SerializeField] protected Transform root;
        protected List<T> list;
        public override void Initialize()
        {
            list = new List<T>();
            root = root ?? transform;
            eventer.Add("Spawn", SpawnHandler);
            eventer.Add("Clear", ClearHandler);
            eventer.Add("DestroyFirst", DestroyFirstHandler);
            eventer.Add("DestroyLast", DestroyLastHandler);
        }
        public override void Dispose()
        {
            eventer.Remove("Spawn", SpawnHandler);
            eventer.Remove("Clear", ClearHandler);
            eventer.Remove("DestroyFirst", DestroyFirstHandler);
            eventer.Remove("DestroyLast", DestroyLastHandler);
            ClearHandler();
            base.Dispose();
        }
        virtual protected void ClearHandler()
        {
            while (list.Count > 0)
            {
                list[0].Dispose();
                list.RemoveAt(0);
            }
        }
        virtual protected void DestroyFirstHandler()
        {
            list[0].Dispose();
            list.RemoveAt(0);
        }
        virtual protected void DestroyLastHandler()
        {
            list[list.Count-1].Dispose();
            list.RemoveAt(list.Count - 1);
        }
        virtual protected void SpawnHandler()
        {
            T item = Instantiate<T>(prefab, root);
            list.Add(item);
        }
    }
}