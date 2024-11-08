using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace LucidFactory.Cards
{
    /// <summary>
    /// Handles a hand of the player
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class Hand<T> : CardCollection<T> where T : ICard
    {
        public event Action OnCardOrderChanged;

        [ShowInInspector, HideInEditorMode]
        public List<T> CardsList { get; private set; }
        public sealed override IEnumerable<T> Cards => CardsList;

        public Hand(int maxSize, params T[] cardsArray) : base(maxSize, cardsArray)
        {
            CardsList = new List<T>();
        }

        public Hand(int maxSize, IEnumerable<T> cards) : base(maxSize, cards)
        {
            CardsList = new List<T>();
        }



        /// <summary>
        /// Adds a given card to the hand
        /// </summary>
        /// <param name="card">Card to add to the hand</param>
        protected override void AddCard(T card)
        {
            CardsList.Add(card);
        }

        protected override void RemoveCard(T card)
        {
            CardsList.Remove(card);
        }

        public override bool HasCard(T card)
        {
            return CardsList.Contains(card);
        }

        public virtual int GetCardIndex(T card) => CardsList.IndexOf(card);

        public virtual bool SetCardIndex(T card, int index)
        {
            if (HasCard(card) && index >= 0 && index < Size)
            {
                CardsList.Remove(card);
                CardsList.Insert(index, card);
                OnCardOrderChanged?.Invoke();
                return true;
            }

            return false;
        }
    }
}