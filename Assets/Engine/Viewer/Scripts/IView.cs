using System;
using System.Threading;
using System.Threading.Tasks;

namespace StateEngine.Views
{
    public interface IView : IDisposable
    {
        /// <summary>
        /// Overlay views are shown on top of the current view without closing it.
        /// </summary>
        bool IsOverlay { get; }

        bool IsFocused { get; }

        Task FocusInAsync(CancellationToken cancellationToken = default);

        Task FocusOutAsync(CancellationToken cancellationToken = default);
    }
}
