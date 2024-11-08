using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace LucidFactory.Cards
{
    /// <summary>
    /// Handles a deck of the player
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class Deck<T> : CardCollection<T> where T : ICard
    {
        public Queue<T> CardsQueue { get; private set; }
        public override IEnumerable<T> Cards => CardsQueue;
        public event Action OnCardShuffled;
        public event Action<T> OnCardDrawn;

        public Deck(params T[] cards) : base(-1, cards)
        {
            CardsQueue = new Queue<T>();
        }

        public Deck(IEnumerable<T> cards) : base(-1, cards)
        {
            CardsQueue = new Queue<T>();
        }

        /// <summary>
        /// Adds a given card to the deck
        /// </summary>
        /// <param name="card">Card to add to the deck</param>
        protected override void AddCard(T card)
        {
            CardsQueue.Enqueue(card);
        }

        /// <summary>
        /// Removes a card from the deck
        /// /!\ Try to use TryDrawCard() instead, expensive function
        /// </summary>
        /// <param name="card">Card to remove</param>
        protected override void RemoveCard(T card)
        {
            List<T> tmp = new List<T>(CardsQueue);
            tmp.Remove(card);

            CardsQueue = new Queue<T>(tmp);
        }

        public override bool HasCard(T card)
        {
            return CardsQueue.Contains(card);
        }

        /// <summary>
        /// Tries to draw a card
        /// </summary>
        /// <param name="card">out Card drawn</param>
        /// <returns>True if a card was succesfully drawn (passed through the out), false otherwise</returns>
        public bool TryDrawCard(out T card)
        {
            // Tries to remove the card from the deck
            if (!CardsQueue.TryDequeue(out card)) return false;

            OnCardDrawn?.Invoke(card);
            return true;
        }

        /// <summary>
        /// Tries to get the next card (doesn't remove it, only checks what card it is)
        /// </summary>
        /// <param name="card">out next card found</param>
        /// <returns>True if a card was found, false otherwise</returns>
        public bool TryGetNextCard(out T card)
        {
            return CardsQueue.TryPeek(out card);
        }

        /// <summary>
        /// Shuffles the deck
        /// </summary>
        public void Shuffle()
        {
            // Here we need to create a temporary deck to shuffle it and then reinject it in the actual deck because queues cannot be shuffled

            // Copy the deck
            List<T> tmp = new List<T>(CardsQueue);

            // shuffle the copied deck
            tmp.Sort((_, _) => Random.value.CompareTo(Random.value));

            // Empty the deck
            CardsQueue.Clear();

            //for each card in the copied deck
            foreach (T card in tmp)
            {
                // Adds it to the current deck
                CardsQueue.Enqueue(card);
            }
            OnCardShuffled?.Invoke();
        }
    }
}