using StateEngine.Models;
using StateEngine.Views;

namespace StateEngine.States
{
    public readonly struct StateContext
    {
        public StateContext(IStater stater, IModel model, IModel primaryModel, IViewer viewer)
        {
            Stater = stater;
            Model = model;
            PrimaryModel = primaryModel;
            Viewer = viewer;
        }

        public IStater Stater { get; }

        public IModel Model { get; }

        public IModel PrimaryModel { get; }

        public IViewer Viewer { get; }
    }
}
