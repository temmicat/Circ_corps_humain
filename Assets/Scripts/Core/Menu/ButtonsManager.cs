using UnityEngine;
using UnityEngine.SceneManagement;

namespace CorpsHumain.Core
{
    public class ButtonsManager : MonoBehaviour
    {
        [Header("Buttons")]
        public GameObject clearConfirmButton;
        public GameObject quitConfirmButton;
        public GameObject levelConfirmButton;
        public GameObject validateResultsButton;
        public GameObject backButton;

        [Header("Panels")]
        public GameObject settingsPanel;
        public GameObject selectionPanel;
        public GameObject ResultPanel;

        [Header("Scripts")]
        public WinSystem winSystem;
        public SetLevelsCleared setLevelsCleared;


        // Need to access the Scriptable object GameData
        [SerializeField] GameData gameDataScriptable;

        private void Start()
        {
            if (SceneManager.GetActiveScene().name == "MainMenu")
                setLevelsCleared.CheckValues();
            if (gameDataScriptable.levelsCleared.Count == 13)
            {
                ResultPanel.SetActive(true);
                selectionPanel.SetActive(false);
            }
        }

        #region SelectionPanel
        public void ClearButton()
        {
            // set active ClearConfirmButton
            clearConfirmButton.SetActive(true);
        }

        public void ClearConfirmButton()
        {
            // Reset GameData
            gameDataScriptable.levelsCleared.Clear();
            gameDataScriptable.playerAnswers.Clear();

            setLevelsCleared.SetButtonsActive();

            clearConfirmButton.SetActive(false);
        }

        public void SettingsButton()
        {
            // Set Active SettingsPanel
            settingsPanel.SetActive(true);
            // Set Unactive SelectionPanel
            selectionPanel.SetActive(false);
        }

        public void QuitButton()
        {
            // SetActive QuitConfirmButton
            quitConfirmButton.SetActive(true);
        }

        public void QuitConfirmButton()
        {
            // Quit game
            quitConfirmButton.SetActive(false);
            Application.Quit();
        }

        public void ChooseLevel(string levelStr)
        {
            if (System.Enum.TryParse(levelStr, out GameData.levels level))
            {
                ChooseOrgan(level);
            }
        }

        private void ChooseOrgan(GameData.levels level)
        {
            // Select one organ from GameData.levelsList
            gameDataScriptable.levelActive = level;
            levelConfirmButton.SetActive(true);
            Debug.Log(level);
            Debug.Log(gameDataScriptable.levelActive);
        }

        public void LevelConfirmButton()
        {
            // change scene
            levelConfirmButton.SetActive(false);
            SceneManager.LoadScene(1);
        }
        #endregion SelectionPanel

        #region SettingsPanel
        public void EnglishButton()
        {
            // Set every texts in english ? (maybe make other scenes in english)
        }

        public void FrenchButton()
        {
            // Set every texts in french
        }

        public void SelctionButton()
        {
            // Set Active selectionPanel
            selectionPanel.SetActive(true);
            // Set UnActive settingsPanel
            settingsPanel.SetActive(false);
        }
        #endregion SettingsPanel

        #region GamePanel
        public void ValidateResultsButton()
        {
            if(gameDataScriptable.playerAnswers.Count == gameDataScriptable.answersNumber)
            {
                gameDataScriptable.levelsCleared.Add(gameDataScriptable.levelActive);
                validateResultsButton.SetActive(false);
                backButton.SetActive(true);
                winSystem.GetResults();
            }
        }

        public void BackButton()
        {
            SceneManager.LoadScene(0);
        }
        #endregion GamePanel
    }
}
