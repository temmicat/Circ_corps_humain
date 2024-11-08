using System;

#if UNITY_EDITOR
using DG.DOTweenEditor;
#endif

using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace LucidFactory.UI.Panels
{
    public interface IPanelEventReceiver
    {
        public void OnPanelOpen();
        public void OnPanelClose();
    }
    
    [Serializable]
    public class LF_Tab
    {
        [SerializeField, BoxGroup("Base")]
        public string name;
        [SerializeField, BoxGroup("Base")]
        private CanvasGroup canvasGroup;

        [SerializeField, FoldoutGroup("Animation")]
        private bool animateCanvasGroup = true;
        
        [SerializeField, FoldoutGroup("Animation"), ShowIf(nameof(animateCanvasGroup))] 
        private float fadeIn;
        [SerializeField, FoldoutGroup("Animation"), ShowIf(nameof(animateCanvasGroup))] 
        private float fadeOut;
        [SerializeField, FoldoutGroup("Animation"), ShowIf(nameof(animateCanvasGroup))]
        private bool doScale;

        [SerializeField, FoldoutGroup("Other")]
        private bool triggerEvents = false;
        [SerializeField, FoldoutGroup("Other")] 
        private bool setIsActive;
        
        public void Open() => PerformCloseOrOpen(true);
        public void Close() => PerformCloseOrOpen(false);

        private void PerformCloseOrOpen(bool open)
        {
            if(open && setIsActive)
                canvasGroup.gameObject.SetActive(true);
            
            if (animateCanvasGroup)
                AnimateCanvasGroup(open);
            else
            {
                if (triggerEvents)
                    TriggerEvents(open);
            }
        }
        private void AnimateCanvasGroup(bool open)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            
            canvasGroup.DOKill();
            float fade = open ? fadeIn : fadeOut;
            var mainTween = DOTween.To(GetAlpha, SetAlpha, open ? 1 : 0, fade)
                .SetTarget(canvasGroup)
                .SetUpdate(true)
                .ChangeStartValue(open ? 0 : GetAlpha())
                .OnComplete(open ? Activate : Deactivate);

#if UNITY_EDITOR
            if (!Application.isPlaying)
                DOTweenEditorPreview.PrepareTweenForPreview(mainTween, false);
#endif
            if (doScale)
            {
                const float minScale = .9f;
                var scaleTween = canvasGroup.transform.DOScale(open ? 1 : minScale, fade)
                    .ChangeStartValue(open ? Vector3.one * minScale : Vector3.one)
                    .SetUpdate(true)
                    .SetTarget(canvasGroup);
                    
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DOTweenEditorPreview.PrepareTweenForPreview(scaleTween, false);
#endif
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DOTweenEditorPreview.Start();
#endif
        }
        private void TriggerEvents(bool open)
        {
            IPanelEventReceiver[] eventReceivers = canvasGroup.GetComponents<IPanelEventReceiver>();
            for (int i = 0; i < eventReceivers.Length; i++)
            {
                if(open)
                    eventReceivers[i].OnPanelOpen();
                else
                    eventReceivers[i].OnPanelClose();
            }
        }

        private float GetAlpha()
        {
            return canvasGroup.alpha;
        }

        private void SetAlpha(float value)
        {
            canvasGroup.alpha = value;
        }

        private void Activate()
        {
            if(setIsActive)
                canvasGroup.gameObject.SetActive(true);

            if (animateCanvasGroup)
            {
                canvasGroup.alpha = 1;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;   
            }
        }
        
        private void Deactivate()
        {
            if(setIsActive)
                canvasGroup.gameObject.SetActive(false);

            if (animateCanvasGroup)
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
    }
}
