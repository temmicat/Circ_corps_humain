using UnityEngine;

namespace CorpsHumain.Core
{
    [DefaultExecutionOrder(-100)]
    public class Player : MonoBehaviour
    {
        public DefenceCardHand cardHand;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
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
