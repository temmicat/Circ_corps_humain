using NUnit.Framework;
using UnityEngine;

namespace CorpsHumain.Core
{
    public class SetLevelsCleared : MonoBehaviour
    {
        public GameData gameDataScriptable;

        [Header("Buttons")]
        public GameObject colDeLUterusButton;
        public GameObject colonRectumButton;
        public GameObject endometreButton;
        public GameObject estomacButton;
        public GameObject foieButton;
        public GameObject oesophageButton;
        public GameObject ovairesButton;
        public GameObject pancreasButton;
        public GameObject peauButton;
        public GameObject poumonsButton;
        public GameObject reinButton;
        public GameObject seinButton;
        public GameObject vessieButton;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
                CheckValues();
        }

        private void CheckValues()
        {
            if(gameDataScriptable.levelsCleared.Contains(GameData.levels.ColDeLUterus))
            {
                colDeLUterusButton.SetActive(false);
            }
            if (gameDataScriptable.levelsCleared.Contains(GameData.levels.ColonRectum))
            {
                colonRectumButton.SetActive(false);
            }
            if (gameDataScriptable.levelsCleared.Contains(GameData.levels.Endometre))
            {
                endometreButton.SetActive(false);
            }
            if (gameDataScriptable.levelsCleared.Contains(GameData.levels.Estomac))
            {
                estomacButton.SetActive(false);
            }
            if (gameDataScriptable.levelsCleared.Contains(GameData.levels.Foie))
            {
                foieButton.SetActive(false);
            }
            if (gameDataScriptable.levelsCleared.Contains(GameData.levels.Oesophage))
            {
                oesophageButton.SetActive(false);
            }
            if (gameDataScriptable.levelsCleared.Contains(GameData.levels.Ovaires))
            {
                ovairesButton.SetActive(false);
            }
            if (gameDataScriptable.levelsCleared.Contains(GameData.levels.Pancreas))
            {
                pancreasButton.SetActive(false);
            }
            if (gameDataScriptable.levelsCleared.Contains(GameData.levels.Peau))
            {
                peauButton.SetActive(false);
            }
            if (gameDataScriptable.levelsCleared.Contains(GameData.levels.Poumon))
            {
                poumonsButton.SetActive(false);
            }
            if (gameDataScriptable.levelsCleared.Contains(GameData.levels.Rein))
            {
                reinButton.SetActive(false);
            }
            if (gameDataScriptable.levelsCleared.Contains(GameData.levels.Sein))
            {
                seinButton.SetActive(false);
            }
            if (gameDataScriptable.levelsCleared.Contains(GameData.levels.Vessie))
            {
                vessieButton.SetActive(false);
            }
        }
    }
}
