using UnityEngine;
using UnityEngine.UI;

namespace CorpsHumain.Core
{
    public class WinSystem : MonoBehaviour
    {
        public GameData gameDataScriptable;
        public OrganeUI organUI;
        public GameObject organHand;

        public void GetResults()
        {
            bool thisCardIsGoodAnswer = false;
            // Compare results to organ data 
            Debug.Log(organUI.organeDataScriptable.numberOfAnswers);
            for (int answerNumber = 0; answerNumber < organUI.organeDataScriptable.numberOfAnswers; answerNumber++) 
            {
                for (int cardNumber = 0; cardNumber < organUI.organeDataScriptable.numberOfAnswers; cardNumber++)
                {
                    if (gameDataScriptable.playerAnswers[answerNumber] == organUI.organeDataScriptable.thisOrganAnswers[cardNumber])
                    {
                        // Set This card to valid
                        thisCardIsGoodAnswer |= true;
                    }
                    //
                }
                ResultUI(answerNumber, thisCardIsGoodAnswer);
                thisCardIsGoodAnswer = false ;
            }
            gameDataScriptable.playerAnswers.Clear();
        }

        private void ResultUI(int i, bool thisCardIsGoodAnswer)
        {
            GameObject card = organHand.transform.GetChild(i).gameObject;
            Image cardImage = card.transform.GetChild(0).gameObject.GetComponent<Image>();
            if (thisCardIsGoodAnswer)
            {
                Debug.Log("True");
                cardImage.color = Color.green;
            }
            else
            {
                cardImage.color = Color.red;
                Debug.Log("Falseeee");
            }
        }
    }
}
