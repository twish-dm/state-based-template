namespace StateEngine.States
{
    using StateEngine.Model;
    using StateEngine.Events;
    using StateEngine.Views;

    public interface IState : IInitialize
    {
        IModel InternalModel {get; set;}
        IModel MainModel { get; set; }
        IEventer Eventer { get; }
        IStater Stater { get; set; }
        IViewer Viewer { get; set; }
        string Name { get; }
        void Enter();
        void Exit();
        void Send(string type);
        void Send<T>(string type, T eventData) where T : IEventData;
    }
}