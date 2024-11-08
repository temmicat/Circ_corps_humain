using System;
using System.Collections;
using LucidFactory.Cards.UI.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LucidFactory.Cards.UI.Hand
{
    [RequireComponent(typeof(LayoutElement)), RequireComponent(typeof(RequireComponent)), RequireComponent(typeof(CanvasGroup))]
    public class CardContainer : MonoBehaviour
    {
        private event Action<RectTransform> OnRectChanged;

        private LayoutElement layoutElement;
        protected ICardUI CardUI { get; private set; }
        protected RectTransform RectTransform { get; private set; }

        [ShowInInspector]
        public Vector3 Position => RectTransform.position;

        [ShowInInspector]
        public Vector3 AnchoredPosition3D => RectTransform.anchoredPosition3D;

        [ShowInInspector]
        public Vector2 AnchoredPosition => RectTransform.anchoredPosition;

        [ShowInInspector]
        public Quaternion Rotation => RectTransform.rotation;

        private int refCount = 0;
        protected virtual void Awake()
        {
            layoutElement = GetComponent<LayoutElement>();
            if (layoutElement == null)
                layoutElement = gameObject.AddComponent<LayoutElement>();

            RectTransform = transform as RectTransform;
            layoutElement.ignoreLayout = true;
        }
        private void OnRectTransformDimensionsChange() => OnRectChanged?.Invoke(RectTransform);

        internal void ConnectToCard(ICardUI cardUI)
        {
            this.CardUI = cardUI;
            AddReference();
        }

        public virtual void ApplyLayoutToCard(CardStateSettings settings)
        {
            StopAllCoroutines();
            // StartCoroutine(ScaleCoroutine(target, settings.TransitionDuration));
        }

        private IEnumerator ScaleCoroutine(RectTransform target, float animationDuration)
        {
            float animationTime = 0;

            Vector3 startAnchors = target.anchoredPosition3D;
            Vector2 startSizeDelta = target.sizeDelta;

            while (animationTime < animationDuration)
            {
                float ctx = animationDuration / animationDuration;

                target.anchoredPosition3D = Vector3.Lerp(startAnchors, RectTransform.anchoredPosition3D, ctx);
                target.sizeDelta = Vector3.Lerp(startSizeDelta, RectTransform.sizeDelta, ctx);
                target.ForceUpdateRectTransforms();
                animationTime += Time.unscaledTime;

                yield return null;
            }
            target.anchoredPosition3D = RectTransform.anchoredPosition3D;
            target.sizeDelta = RectTransform.sizeDelta;

            target.ForceUpdateRectTransforms();
        }
        public virtual void Activate(CardStateSettings settings)
        {
            RectTransform.ForceUpdateRectTransforms();
            SetLayoutSettings(settings.ActiveLayout);
        }

        public virtual void Deactivate(CardStateSettings settings)
        {
            SetLayoutSettings(settings.InactiveLayout);
        }

        private void SetLayoutSettings(CardStateSettings.LayoutSettings layoutSettings)
        {
            if (layoutSettings.IgnoreLayout)
                layoutElement.ignoreLayout = true;
            else
            {
                layoutElement.ignoreLayout = false;

                layoutElement.minWidth = layoutSettings.HasMin ? layoutSettings.Min.x : -1f;
                layoutElement.minHeight = layoutSettings.HasMin ? layoutSettings.Min.y : -1f;

                layoutElement.preferredWidth = layoutSettings.HasPreferred ? layoutSettings.Preferred.x : -1f;
                layoutElement.preferredHeight = layoutSettings.HasPreferred ? layoutSettings.Preferred.y : -1f;

                layoutElement.flexibleWidth = layoutSettings.HasFlexible ? layoutSettings.Flexible.x : -1f;
                layoutElement.flexibleHeight = layoutSettings.HasFlexible ? layoutSettings.Flexible.y : -1f;
            }
        }

        public void AddReference()
        {
            refCount++;
        }

        public void RemoveReference()
        {
            refCount--;
            if(refCount <= 0)
                Destroy(gameObject);
        }
    }
}