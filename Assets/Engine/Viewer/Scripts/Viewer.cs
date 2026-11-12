using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StateEngine.Models;
using UnityEngine;

namespace StateEngine.Views
{
    /// <summary>
    /// Stack-based screen navigation. Views are loaded from Resources by name and destroyed when closed.
    /// Transitions are queued, so calls made during an animation run after it finishes.
    /// </summary>
    [DefaultExecutionOrder(-800)]
    public class Viewer : MonoBehaviour, IViewer
    {
        [SerializeField, Tooltip("Folder inside Resources that contains view prefabs.")]
        private string m_ResourcesPath = "Views";

        private readonly Stack<string> m_History = new Stack<string>();
        private readonly Dictionary<string, IView> m_OpenViews = new Dictionary<string, IView>();
        private Task m_Transition = Task.CompletedTask;

        public IView CurrentView =>
            m_History.Count > 0 && m_OpenViews.TryGetValue(m_History.Peek(), out IView view) ? view : null;

        protected virtual void Awake()
        {
            ServiceRegistry.Register<IViewer>(this);
        }

        protected virtual void OnDestroy()
        {
            ServiceRegistry.Unregister<IViewer>(this);
        }

        public Task Push(string viewName, bool clearHistory = true) => Enqueue(() => PushInternal(viewName, clearHistory));

        public Task Pop() => Enqueue(PopInternal);

        public Task PopAll() => Enqueue(PopAllInternal);

        protected virtual IView CreateView(string viewName)
        {
            var prefab = Resources.Load<GameObject>($"{m_ResourcesPath}/{viewName}");
            if (prefab == null)
                throw new ArgumentException($"View prefab '{m_ResourcesPath}/{viewName}' was not found in Resources.", nameof(viewName));

            GameObject instance = Instantiate(prefab, transform);
            instance.name = viewName;

            if (!instance.TryGetComponent(out IView view))
            {
                Destroy(instance);
                throw new InvalidOperationException($"Prefab '{viewName}' has no component implementing {nameof(IView)}.");
            }

            return view;
        }

        private async Task PushInternal(string viewName, bool clearHistory)
        {
            if (!clearHistory && m_History.Count > 0 && m_History.Peek() == viewName)
                return;

            IView next = GetOrCreate(viewName);

            if (clearHistory)
                await CloseAllExcept(viewName);
            else if (!next.IsOverlay && m_History.Count > 0)
                await Close(m_History.Peek());

            if (clearHistory)
                m_History.Clear();

            m_History.Push(viewName);
            await next.FocusInAsync(destroyCancellationToken);
        }

        private async Task PopInternal()
        {
            if (m_History.Count == 0)
                return;

            await Close(m_History.Pop());

            if (m_History.Count > 0)
            {
                IView previous = GetOrCreate(m_History.Peek());
                if (!previous.IsFocused)
                    await previous.FocusInAsync(destroyCancellationToken);
            }
        }

        private async Task PopAllInternal()
        {
            await CloseAllExcept(null);
            m_History.Clear();
        }

        private IView GetOrCreate(string viewName)
        {
            if (!m_OpenViews.TryGetValue(viewName, out IView view))
            {
                view = CreateView(viewName);
                m_OpenViews.Add(viewName, view);
            }

            return view;
        }

        private async Task Close(string viewName)
        {
            if (!m_OpenViews.TryGetValue(viewName, out IView view))
                return;

            m_OpenViews.Remove(viewName);
            await view.FocusOutAsync(destroyCancellationToken);
            view.Dispose();
        }

        private async Task CloseAllExcept(string keptViewName)
        {
            var viewNames = new List<string>(m_OpenViews.Keys);
            foreach (string viewName in viewNames)
            {
                if (viewName != keptViewName)
                    await Close(viewName);
            }
        }

        private Task Enqueue(Func<Task> transition)
        {
            m_Transition = RunAfter(m_Transition, transition);
            return m_Transition;
        }

        private static async Task RunAfter(Task previous, Func<Task> transition)
        {
            await previous;

            try
            {
                await transition();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }
}
