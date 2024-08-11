using System.Collections.Generic;

namespace StateEngine.Views
{
    public interface IViewer
    {
        IView CurrentView { get; }
        IView OverlayView { get; }
        void Push(string name, bool popAll);
        void Push(string name);
        void PopAll();
        void Pop();
    }
}