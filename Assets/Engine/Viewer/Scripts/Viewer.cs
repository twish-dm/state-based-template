namespace StateEngine.Views
{
    using StateEngine.Model;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    [DefaultExecutionOrder(-801)]
    public class Viewer : MonoBehaviour, IViewer
    {
        public const string KEY = "viewer";
        virtual protected void Awake()
        {
            StaticModel.Add(KEY, this);
        }
        virtual protected void OnDestroy()
        {
            StaticModel.Remove(KEY);
        }
        protected Stack<string> viewsStack = new Stack<string>();
        protected Dictionary<string, IView> viewsMap = new Dictionary<string, IView>();
        public IView CurrentView { get; protected set; }
        public IView OverlayView { get; protected set; }
        async virtual public void Push(string name, Dictionary<string, object> data, bool popAll)
        {
            if (popAll)
                PopAll();

            if (CurrentView != null && !CurrentView.IsOverlay)
            {
                await CurrentView.FocusOut();
                Remove(CurrentView.name);
            }
            CurrentView = Add(name);
            if (CurrentView.IsOverlay)
            {
                OverlayView = CurrentView;
            }
            viewsStack.Push(name);
            await CurrentView.FocusIn();
        }
        async virtual public void Pop()
        {
            if (CurrentView != null && !CurrentView.IsOverlay)
            {

                await CurrentView.FocusOut();
                Remove(CurrentView.name);
                viewsStack.Pop();
                CurrentView = null;
            }
            if (CurrentView == null && viewsStack.TryPeek(out string peek))
            {
                CurrentView = Add(peek);
                await CurrentView.FocusIn();
            }
        }
        virtual protected void Remove(string name)
        {
            if (viewsMap.ContainsKey(name))
            {
                viewsMap[name].Dispose();
                viewsMap.Remove(name);
            }
        }
        virtual protected IView Add(string name)
        {
            IView view = null;
            if (!viewsMap.ContainsKey(name))
            {
                view = LoadView(name);
                viewsMap.Add(name, view);
            }
            else
            {
                view = viewsMap[name];
            }
            return view;
        }
        virtual public void Push(string name, bool popAll)
        {
            Push(name, null, popAll);
        }
        virtual public void Push(string name)
        {
            Push(name, null, true);
        }
        async virtual public void PopAll()
        {
            if (CurrentView != null && !CurrentView.IsOverlay)
            {
                await CurrentView.FocusOut();
                Remove(CurrentView.name);
                viewsStack.Clear();
            }
        }

        virtual protected IView LoadView(string name)
        {
            Debug.Log(name);
            GameObject viewGameObject = Instantiate(Resources.Load<GameObject>($"Views/{name}"), transform);
            viewGameObject.name = viewGameObject.name.Replace("(Clone)", "");
            IView view = viewGameObject.GetComponent<IView>();
            
            return view;
        }
    }
}