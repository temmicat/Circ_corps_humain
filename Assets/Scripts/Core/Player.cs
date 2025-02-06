using LTX.Singletons;
using UnityEngine;

namespace CorpsHumain.Core
{
    [DefaultExecutionOrder(-100)]
    public class Player : MonoSingleton<Player>
    {

        // This Script Creates the DefenceCardHand, based on the Cards located under Resources -> Cards

        public DefenceCardHand cardHand;
        public DefenceCardHand organHand;

        [SerializeField]
        public DefenceCardHandUI handUI;
        [SerializeField]
        public DefenceCardHandUI organhandUI;

        protected override void Awake()
        {
            base.Awake();
            Debug.Log("Player");

            cardHand = new DefenceCardHand(12);
            organHand = new DefenceCardHand(12);
            handUI.Bind(cardHand);
            organhandUI.Bind(organHand);


            DefenceCardData[] data = Resources.LoadAll<DefenceCardData>("Cards");
            // Instantiate Organe where thisOrgane == GameData.levelActive

            for (int i = 0; i < data.Length; i++)
            {
                DefenceCard card = new DefenceCard(data[i]);
                card.isPlayerCard = true;

                cardHand.TryAddCard(card);
            }
        }


        public void DropCardOnOrgan(DefenceCard card)
        {
            cardHand.TryRemoveCard(card);
            organHand.TryAddCard(card);
            card.isPlayerCard = false;
        }

        public void RemoveCardOfOrgan(DefenceCard card)
        {
            organHand.TryRemoveCard(card);
            cardHand.TryAddCard(card);
            card.isPlayerCard = true;
        }
    }
}
