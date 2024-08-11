using System;
using System.Threading.Tasks;

namespace StateEngine.Views
{
    public interface IView:IDisposable
    {
        public string name { get; }
        bool IsOverlay { get; set; }
        bool IsFocused { get; }
        Task FocusIn();
        Task FocusOut();
    }
}