using System.Collections.Generic;
using System.Threading.Tasks;

using DG.Tweening;

using StateEngine.Behaviours;

using UnityEngine;
using UnityEngine.UI;

namespace StateEngine.Views
{
    public class ViewBase : DynamicBehaviour, IView
    {
        [field: SerializeField] virtual public bool IsOverlay { get; set; }
        protected CanvasGroup canvasGroup;
        virtual public bool IsFocused { get; protected set; }

        virtual protected void OnEnable()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
        }
        virtual protected void OnDisable()
        {
        }
        async virtual public Task FocusIn()
        {
            IsFocused = true;
            gameObject.SetActive(true);

            transform.SetAsLastSibling();
            await canvasGroup.DOFade(1f, 0.2f).AsyncWaitForCompletion();
        }
        async virtual public Task FocusOut()
        {
            IsFocused = false;
            await canvasGroup.DOFade(0f, 0.2f).AsyncWaitForCompletion();
            await Task.CompletedTask;
        }

        public override void Initialize()
        {
        }
    }
}