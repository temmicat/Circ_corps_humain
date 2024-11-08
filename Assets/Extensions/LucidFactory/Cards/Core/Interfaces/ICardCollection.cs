using System;
using System.Collections.Generic;
using LucidFactory.Cards.UI.Interfaces;

namespace LucidFactory.Cards
{
    public interface ICardCollection<out T> where T : ICard
    {
        public event Action OnCleared;
        public event Action<T> OnCardAdded;
        public event Action<T> OnCardRemoved;

        public IEnumerable<T> Cards { get; }

    }
}