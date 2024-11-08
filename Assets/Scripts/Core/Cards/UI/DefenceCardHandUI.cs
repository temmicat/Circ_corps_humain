using UnityEngine;
using LucidFactory.Cards.UI;
using LucidFactory.Cards.UI.Hand;
using System.Collections.Generic;
using LucidFactory.Cards.UI.Interfaces;
using System;

namespace CorpsHumain.Core
{
    public class DefenceCardHandUI : HandUI<DefenceCard, DefenceCardHand>
    {

        [SerializeField]
        private GameObject prefab;

        /*
        [SerializeField]
        DefenceCardHandUI handUI;
        [SerializeField]
        Player player;

        private void Awake()
        {
            handUI.Bind(player.cardHand);

        }
        */

        protected override void OnBind(DefenceCardHand playerCardCollection)
        {

        }

        protected override void OnUnbind(DefenceCardHand playerCardCollection)
        {

        }

        protected override bool TryCreateCardUIForCard(DefenceCard card, out ICardUI<DefenceCard> createdCard)
        {
            createdCard = Instantiate(prefab, transform).GetComponent<DefenceCardUI>();

            return true;
        }

        public override IEnumerable<ICardDropSlot> GetSlots(ICardUI cardUI)
        {
            return Array.Empty<ICardDropSlot>();
        }
    }
}
