using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CorpsHumain.Core
{
    public class DefenceCardHand
    {
        public event Action<DefenceCard> onCardAdded;
        public event Action<DefenceCard> onCardRemoved;

        private List<DefenceCard> cards;
        public int MaxSize { get; }

        public DefenceCardHand(int maxSize)
        {
            cards = new List<DefenceCard>();
            MaxSize = maxSize;
        }

        public bool TryAddCard(DefenceCard card) 
        {
            if (cards.Count < MaxSize)
            {
                cards.Add(card);
                onCardAdded?.Invoke(card);
                return true;
            }

            return false;
        }

        public bool TryRemoveCard(DefenceCard card)
        {
            if(cards.Remove(card))
            {
                onCardRemoved?.Invoke(card);
                return true;
            }

            return false;
        }

    }
}
