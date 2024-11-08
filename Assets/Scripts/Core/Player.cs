using UnityEngine;

namespace CorpsHumain.Core
{
    public class Player : MonoBehaviour
    {
        public DefenceCardHand cardHand;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            cardHand = new DefenceCardHand(12);
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
