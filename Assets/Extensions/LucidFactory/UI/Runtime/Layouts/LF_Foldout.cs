using System;
using UnityEngine;

using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace LucidFactory.UI
{
    [RequireComponent(typeof(RectTransform)),
     AddComponentMenu("LucidFactory/Layout/Foldout"),
     ExecuteInEditMode]
    public class LF_Foldout : MonoBehaviour, ILayoutController, ICanvasElement
    {
        public event Action<LF_Foldout> OnOpened;
        public event Action<LF_Foldout> OnClosed;

        [SerializeField, BoxGroup("Settings")]
        private Ease ease;
        [SerializeField, BoxGroup("Settings")]
        private float foldoutAnimationTime;
        [SerializeField, BoxGroup("Runtime"), OnValueChanged(nameof(SyncWithIsOpen))]
        private bool isOpen;

        [SerializeField, ReadOnly, BoxGroup("Runtime")]
        private float lerp;

        [SerializeField, BoxGroup("Rects"), Required(InfoMessageType.Warning)]
        private RectTransform targetRect;
        [Space]
        [SerializeField, BoxGroup("Rects")]
        private RectTransform closedRect;
        [SerializeField, BoxGroup("Rects")]
        private RectTransform openedRect;

        private DrivenRectTransformTracker drivenRectTransformTracker = new();

        private Tween animationTween;

        public bool IsOpen => isOpen;

        private void Start()
        {
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
                LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
            else
                return;

            drivenRectTransformTracker.Clear();

            if (targetRect != null)
            {
                DrivenTransformProperties properties =
                    DrivenTransformProperties.AnchoredPosition |
                    DrivenTransformProperties.SizeDelta |
                    DrivenTransformProperties.Pivot |
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchorMax|
                    DrivenTransformProperties.AnchorMin;

                drivenRectTransformTracker.Add(this, targetRect, properties);
            }
        }

        private void OnDisable()
        {
            drivenRectTransformTracker.Clear();
#if UNITY_EDITOR
            DG.DOTweenEditor.DOTweenEditorPreview.Stop();
#endif
        }
#endif
#if UNITY_EDITOR
        private void Update()
        {
            if(Application.isPlaying)
                return;

            OnUpdate();
        }
#endif

        public void Open(bool immediate = false)
        {
            if (Application.isPlaying && isOpen)
                return;

            isOpen = true;
            if(animationTween.IsActive())
                animationTween.Kill();

            if (immediate)
            {
                lerp = 1;
                OnUpdate();
                OnComplete();
            }
            else
            {
                float currentProgress = 1 - Mathf.InverseLerp(0, 1, lerp);
                animationTween = DOTween
                    .To(() => lerp, ctx => lerp = ctx, 1, foldoutAnimationTime * currentProgress)
                    .SetEase(ease)
                    .OnUpdate(OnUpdate)
                    .OnComplete(OnComplete)
                    .SetUpdate(true);
    #if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    DG.DOTweenEditor.DOTweenEditorPreview.PrepareTweenForPreview(animationTween, false);
                    DG.DOTweenEditor.DOTweenEditorPreview.Start();
                }
    #endif

            }
            OnOpened?.Invoke(this);
        }

        public void Close(bool immediate = false)
        {
            if (Application.isPlaying && !isOpen)
                return;

            isOpen = false;

            if (animationTween.IsActive())
                animationTween.Kill();
            if (immediate)
            {
                lerp = 1;
                OnUpdate();
                OnComplete();
            }
            else
            {
                float currentProgress = Mathf.InverseLerp(0, 1, lerp);
                animationTween = DOTween
                    .To(() => lerp, ctx => lerp = ctx, 0, foldoutAnimationTime * currentProgress)
                    .SetEase(ease)
                    .OnUpdate(OnUpdate)
                    .OnComplete(OnComplete)
                    .SetUpdate(true);
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    DG.DOTweenEditor.DOTweenEditorPreview.PrepareTweenForPreview(animationTween, false);
                    DG.DOTweenEditor.DOTweenEditorPreview.Start();
                }
#endif
            }

            OnClosed?.Invoke(this);
        }

        private void SyncWithIsOpen()
        {
            if (isOpen)
                Open();
            else
                Close();
        }

        public void Toggle()
        {
            if(isOpen)
                Close();
            else
                Open();
        }

        private void OnUpdate()
        {
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }
        private void OnComplete()
        {
            animationTween = null;
        }

        void ILayoutController.SetLayoutHorizontal()
        {
            if (targetRect != null)
            {
                float sizeDelta = Mathf.Lerp(closedRect.sizeDelta.x, openedRect.sizeDelta.x, lerp);

                float anchoredPosition = Mathf.Lerp(closedRect.anchoredPosition.x, openedRect.anchoredPosition.x, lerp);
                float anchorMin = Mathf.Lerp(closedRect.anchorMin.x, openedRect.anchorMin.x, lerp);
                float anchorMax = Mathf.Lerp(closedRect.anchorMax.x, openedRect.anchorMax.x, lerp);
                float pivot = Mathf.Lerp(closedRect.pivot.x, openedRect.pivot.x, lerp);

                targetRect.anchorMin = new(anchorMin, targetRect.anchorMin.y);
                targetRect.anchorMax = new(anchorMax, targetRect.anchorMax.y);
                targetRect.pivot = new(pivot, targetRect.pivot.y);
                targetRect.anchoredPosition = new(anchoredPosition, targetRect.anchoredPosition.y);
                targetRect.sizeDelta = new Vector2(sizeDelta, targetRect.sizeDelta.y);
                // targetRect.rect.Set(rect.x, rect.y, rect.width, rect.height);
            }
        }

        void ILayoutController.SetLayoutVertical()
        {
            if (targetRect != null)
            {
                float sizeDelta = Mathf.Lerp(closedRect.sizeDelta.y, openedRect.sizeDelta.y, lerp);

                float anchoredPosition = Mathf.Lerp(closedRect.anchoredPosition.y, openedRect.anchoredPosition.y, lerp);
                float anchorMin = Mathf.Lerp(closedRect.anchorMin.y, openedRect.anchorMin.y, lerp);
                float anchorMax = Mathf.Lerp(closedRect.anchorMax.y, openedRect.anchorMax.y, lerp);
                float pivot = Mathf.Lerp(closedRect.pivot.y, openedRect.pivot.y, lerp);

                targetRect.anchorMin = new(targetRect.anchorMin.x,anchorMin);
                targetRect.anchorMax = new(targetRect.anchorMax.x,anchorMax);
                targetRect.pivot = new(targetRect.pivot.x, pivot);
                targetRect.anchoredPosition = new(targetRect.anchoredPosition.x, anchoredPosition);
                targetRect.sizeDelta = new Vector2(targetRect.sizeDelta.x, sizeDelta);
            }
        }

        #region ICanvasElementImplementations

        private bool isDestroyed;

        void ICanvasElement.Rebuild(CanvasUpdate canvasUpdate)
        {
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }

        public void LayoutComplete() { }

        public void GraphicUpdateComplete() { }

        private void OnDestroy()
        {
            isDestroyed = true;
        }

        public bool IsDestroyed()
        {
            return isDestroyed;
        }

        #endregion
    }
}