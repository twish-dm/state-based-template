using StateEngine.Events;

namespace StateEngine.States
{
    public interface IStater
    {
        IState Current { get; }
        void Add(IState state);
        void Remove(IState state);
        void Remove(string stateName);
        void Start(string stateName);
        void Change(string stateName);
        void Invoke(string type);
        void Invoke<T>(string type, T eventData) where T : IEventData;
    }
}