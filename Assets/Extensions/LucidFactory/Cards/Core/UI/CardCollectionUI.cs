using System;
using LTX.ChanneledProperties;
using LucidFactory.Cards.UI.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LucidFactory.Cards.UI
{
    public abstract class CardCollectionUI<T> : CardCollectionUI<T, ICardCollection<T>>
        where T : ICard
    {

    }

    public abstract class CardCollectionUI<T, TU> : MonoBehaviour
        where T : ICard
        where TU : ICardCollection<T>
    {
        public event Action<T> OnCardWasAdded;
        public event Action<T> OnCardWasRemoved;

        public TU CurrentCardCollection { get; internal set; }
        public PrioritisedProperty<float> Alpha { get; private set; }
        public PrioritisedProperty<bool> Interactable { get; private set; }

        protected CanvasGroup CanvasGroup { get; private set; }

        private bool hasCanvasGroup;


        protected abstract void OnBind(TU playerCardCollection);
        protected abstract void OnUnbind(TU playerCardCollection);

        protected abstract void OnCardAdded(T card);
        protected abstract void OnCardRemoved(T card);

        protected virtual void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
            hasCanvasGroup = CanvasGroup != null;

            Alpha = new PrioritisedProperty<float>(1);
            Alpha.AddOnValueChangeCallback(UpdateCanvasGroupAlpha, true);

            Interactable = new PrioritisedProperty<bool>(true);
            Interactable.AddOnValueChangeCallback(UpdateCanvasGroupInteractable, true);
        }

        public void Bind(TU cardCollection)
        {
            if (CurrentCardCollection != null)
            {
                Debug.LogWarning($"[Card Handler UI] Already binded to a cardHandler. Please unbind it first.");
                return;
            }

            CurrentCardCollection = cardCollection;
            CurrentCardCollection.OnCardAdded += InternalOnCardAdded;
            CurrentCardCollection.OnCardRemoved += InternalOnCardRemoved;
            OnBind(cardCollection);
            foreach (var card in cardCollection.Cards)
                OnCardAdded(card);
        }

        public void UnBind(TU cardCollection, bool clear = true)
        {
            if(CurrentCardCollection == null || !CurrentCardCollection.Equals(cardCollection))
                return;

            CurrentCardCollection.OnCardAdded -= InternalOnCardAdded;
            CurrentCardCollection.OnCardRemoved -= InternalOnCardRemoved;
            CurrentCardCollection = default;

            if (clear)
            {
                foreach (var card in cardCollection.Cards)
                    OnCardRemoved(card);
            }

            OnUnbind(cardCollection);
        }


        protected void InternalOnCardAdded(T card)
        {
            try
            {
                OnCardAdded(card);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                OnCardWasAdded?.Invoke(card);
            }
        }

        protected void InternalOnCardRemoved(T card)
        {
            try
            {
                OnCardRemoved(card);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                OnCardWasRemoved?.Invoke(card);
            }
        }

        private void UpdateCanvasGroupAlpha(float alpha)
        {
            if(hasCanvasGroup)
                CanvasGroup.alpha = alpha;
        }

        private void UpdateCanvasGroupInteractable(bool interactable)
        {
            if (!hasCanvasGroup)
                return;
            CanvasGroup.interactable = interactable;
            CanvasGroup.blocksRaycasts = interactable;
        }
    }
}