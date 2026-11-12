using System.Collections.Generic;
using StateEngine.Behaviours;
using UnityEngine;

namespace StateEngine.Components
{
    /// <summary>
    /// Spawns and destroys instances of <typeparamref name="T"/> in response to "{Key}/Spawn",
    /// "{Key}/DestroyFirst", "{Key}/DestroyLast" and "{Key}/Clear" events.
    /// </summary>
    public abstract class Spawner<T> : BindingBehaviour where T : Component
    {
        [SerializeField] private T m_Prefab;
        [SerializeField] private Transform m_Root;

        private readonly List<T> m_Instances = new List<T>();

        public IReadOnlyList<T> Instances => m_Instances;

        protected override string KeySuffix => "Spawner";

        private string SpawnEvent => Key + "/Spawn";
        private string DestroyFirstEvent => Key + "/DestroyFirst";
        private string DestroyLastEvent => Key + "/DestroyLast";
        private string ClearEvent => Key + "/Clear";

        protected override void OnInitialize()
        {
            if (m_Root == null)
                m_Root = transform;

            Eventer.Add(SpawnEvent, OnSpawnRequested);
            Eventer.Add(DestroyFirstEvent, DestroyFirst);
            Eventer.Add(DestroyLastEvent, DestroyLast);
            Eventer.Add(ClearEvent, Clear);
        }

        protected override void OnRelease()
        {
            Eventer.Remove(SpawnEvent, OnSpawnRequested);
            Eventer.Remove(DestroyFirstEvent, DestroyFirst);
            Eventer.Remove(DestroyLastEvent, DestroyLast);
            Eventer.Remove(ClearEvent, Clear);

            Clear();
        }

        public virtual T Spawn()
        {
            T instance = Instantiate(m_Prefab, m_Root);
            m_Instances.Add(instance);
            return instance;
        }

        public void DestroyFirst() => DestroyAt(0);

        public void DestroyLast() => DestroyAt(m_Instances.Count - 1);

        public void Clear()
        {
            for (int i = m_Instances.Count - 1; i >= 0; i--)
                DestroyAt(i);
        }

        protected virtual void DestroyAt(int index)
        {
            if (index < 0 || index >= m_Instances.Count)
                return;

            T instance = m_Instances[index];
            m_Instances.RemoveAt(index);

            if (instance != null)
                Destroy(instance.gameObject);
        }

        private void OnSpawnRequested() => Spawn();
    }
}
