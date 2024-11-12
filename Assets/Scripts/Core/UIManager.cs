using UnityEngine;

namespace CorpsHumain.Core
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private Player player;

        [SerializeField]
        public DefenceCardHandUI handUI;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            handUI.Bind(player.cardHand);
        }

        private void OnDestroy()
        {
            handUI.UnBind(player.cardHand);
        }
    }
}
