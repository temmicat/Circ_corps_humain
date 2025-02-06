using UnityEngine;
using System;
using LTX.ChanneledProperties;
using System.Collections.Generic;

namespace CorpsHumain.Core
{
    public class DefenceCardHandUI : MonoBehaviour
    {
        [Space]
        [SerializeField]
        private DefenceCardUI prefab;

        private Dictionary<DefenceCard, DefenceCardUI> cards = new();

        public void Bind(DefenceCardHand hand)
        {
            hand.onCardAdded += Hand_onCardAdded;
            hand.onCardRemoved += Hand_onCardRemoved;
        }

        private void Hand_onCardAdded(DefenceCard card)
        {
            DefenceCardUI instance = GameObject.Instantiate(prefab, transform);
            instance.SetData(card);
            cards.Add(card, instance);
        }

        private void Hand_onCardRemoved(DefenceCard card)
        {
            Destroy(cards[card]);
            cards.Remove(card);
        }

    }
}
