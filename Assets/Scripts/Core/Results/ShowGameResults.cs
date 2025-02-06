using UnityEngine;

namespace CorpsHumain.Core
{
    public class ShowGameResults : MonoBehaviour
    {
        public GameData.levels thisOrgan;
        public GameData gameDataScriptable;

        public void Start()
        { 
            for (int i = 0; i < gameDataScriptable.gameAnswers[thisOrgan].Count; i++)
            {
                GameObject carte = Instantiate(gameDataScriptable.gameAnswers[thisOrgan][i]);
                carte.transform.parent = this.gameObject.transform;
            }
        }

        /*
        GameObject childObject = Instantiate(YourObject) as GameObject;
        childObject.transform.parent = parentObject.transform
        */
    }
}
