using UnityEngine;

namespace CorpsHumain.Core
{
    [DefaultExecutionOrder(-100)]
    public class Player : MonoBehaviour
    {

        // This Script Creates the DefenceCardHand, based on the Cards located under Resources -> Cards

        public DefenceCardHand cardHand;


        void Awake()
        {
            Debug.Log("Player");

            cardHand = new DefenceCardHand(12);

            DefenceCardData[] data = Resources.LoadAll<DefenceCardData>("Cards");

            for (int i = 0; i < data.Length; i++)
            {
                DefenceCard card = new DefenceCard(data[i]);

                cardHand.TryAddCard(card);
            }
        }

    }
}
