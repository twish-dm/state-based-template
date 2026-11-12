using System.Threading;
using System.Threading.Tasks;
using StateEngine.Behaviours;
using UnityEngine;

namespace StateEngine.Views
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ViewBase : DynamicBehaviour, IView
    {
        [SerializeField] private bool m_IsOverlay;
        [SerializeField, Min(0f)] private float m_FadeDuration = 0.2f;

        private CanvasGroup m_CanvasGroup;

        public bool IsOverlay => m_IsOverlay;

        public bool IsFocused { get; private set; }

        protected override void OnInitialize()
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
            SetVisible(false, 0f);
        }

        public virtual async Task FocusInAsync(CancellationToken cancellationToken = default)
        {
            IsFocused = true;
            gameObject.SetActive(true);
            transform.SetAsLastSibling();

            await FadeAsync(1f, cancellationToken);
        }

        public virtual async Task FocusOutAsync(CancellationToken cancellationToken = default)
        {
            IsFocused = false;

            await FadeAsync(0f, cancellationToken);
        }

        private async Task FadeAsync(float targetAlpha, CancellationToken cancellationToken)
        {
            using (var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationToken))
            {
                CancellationToken token = linked.Token;
                float startAlpha = m_CanvasGroup.alpha;

                SetInteractable(false);

                for (float elapsed = 0f; elapsed < m_FadeDuration; elapsed += Time.unscaledDeltaTime)
                {
                    m_CanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / m_FadeDuration);
                    await Task.Yield();
                    token.ThrowIfCancellationRequested();
                }

                SetVisible(targetAlpha > 0f, targetAlpha);
            }
        }

        private void SetVisible(bool isVisible, float alpha)
        {
            m_CanvasGroup.alpha = alpha;
            SetInteractable(isVisible);
        }

        private void SetInteractable(bool isInteractable)
        {
            m_CanvasGroup.interactable = isInteractable;
            m_CanvasGroup.blocksRaycasts = isInteractable;
        }
    }
}
