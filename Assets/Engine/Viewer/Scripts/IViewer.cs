using System.Threading.Tasks;

namespace StateEngine.Views
{
    public interface IViewer
    {
        IView CurrentView { get; }

        Task Push(string viewName, bool clearHistory = true);

        Task Pop();

        Task PopAll();
    }
}
