using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Managers
{
    public class SceneController : MonoBehaviour
    {
        #region CORE

        public Text[] UITextElements { get; private set; }
        public Image[] UIImageElements { get; private set; }

        public Button[] Buttons { get; private set; }
        public Text[] ButtonTexts { get; private set; }
        public Image[] ButtonImages { get; private set; }
        
        public Outline[] Outlines { get; private set; }
        public Image[] Blockers { get; private set; }

        public Image SudokuGridBackground { get; private set; }
        public Image[] GridBlockHighlightImages { get; private set; }

        private AppSettingsManager _settingsManager;

        #endregion

        #region MONO

        private void Awake()
        {
            InitializeManagers();
        }

        #endregion

        #region INITIALIZATION

        public void UpdateUIElements()
        {
            UITextElements = GetUIComponents<Text>("UI");
            UIImageElements = GetUIComponents<Image>("UI");

            Buttons = GetUIComponents<Button>("Buttons");
            ButtonTexts = GetUIComponents<Text>("Buttons");
            ButtonImages = GetUIComponents<Image>("Buttons");

            Outlines = FindObjectsOfType<Outline>();
            Blockers = GetUIComponents<Image>("Blocker");

            SudokuGridBackground = GetUIComponents<Image>("Sudoku")?.FirstOrDefault();
            GridBlockHighlightImages = GetUIComponents<Image>("GridBlock");
        }
        
        private void InitializeManagers()
        {
            _settingsManager = AppSettingsManager.Instance;
        }

        #endregion

        #region CORE LOGIC

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        public void LoadMainScene() => LoadScene(_settingsManager.AppSettingData.NameMainScene);
        public void LoadSudokuScene() => LoadScene(_settingsManager.AppSettingData.NameSudokuScene);
        public void LoadRecordsScene() => LoadScene(_settingsManager.AppSettingData.NameRecordsScene);

        private void LoadScene(string value)
        {
            if (!(SceneManager.GetActiveScene().name == value))
                SceneManager.LoadSceneAsync(value);
        }

        #endregion

        #region GET

        private T[] GetUIComponents<T>(string layerName) where T : Component
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer == -1)
            {
                return new T[0];
            }

            return FindObjectsOfType<T>().Where(obj => obj.gameObject.layer == layer).ToArray();
        }

        #endregion
    }
}