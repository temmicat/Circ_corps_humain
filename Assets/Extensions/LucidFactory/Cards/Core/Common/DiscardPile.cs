using System.Collections.Generic;

namespace LucidFactory.Cards
{
    /// <summary>
    /// Handles a discard pile of the player
    /// </summary>
    /// <typeparam name="T"></typeparam>

    [System.Serializable]
    public class DiscardPile<T> : CardCollection<T> where T : ICard
    {
        public List<T> CardsList { get; private set; }
        public override IEnumerable<T> Cards => CardsList;

        // ReSharper disable ConvertConstructorToMemberInitializers
        public DiscardPile(params T[] cards) : base(-1, cards)
        {
            CardsList = new List<T>();
        }
        public DiscardPile(IEnumerable<T> cards) : base(-1, cards)
        {
            CardsList = new List<T>();
        }

        /// <summary>
        /// Adds a given card to the discard pile
        /// </summary>
        /// <param name="card">Card to add</param>
        protected override void AddCard(T card)
        {
            CardsList.Add(card);
        }

        /// <summary>
        /// Removes a given card from the discard pile if possible
        /// </summary>
        /// <param name="card">Card to remove</param>
        protected override void RemoveCard(T card)
        {
            CardsList.Remove(card);
        }

        public override bool HasCard(T card)
        {
            return CardsList.Contains(card);
        }

        public bool TrySeeLastAddedCard(out T card)
        {
            if (IsEmpty)
            {
                card = default(T);
                return false;
            }

            card = CardsList[^1];
            return true;
        }
    }
}