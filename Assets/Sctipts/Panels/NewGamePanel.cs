using UnityEngine;
using UnityEngine.UI;
using Game.Managers;
using Help.UI;
using UnityEngine.EventSystems;
using System.Linq;

namespace Game.Panels
{
    public class NewGamePanel : MonoBehaviour
    {
        #region CORE

        [Header("Core")]
        [SerializeField] private Button[] levelButtons;
        [SerializeField] private Button choosingButton;

        [Header("Continue Game")]
        [SerializeField] private Button continueGameButton;
        [SerializeField] private Text continueGameText;

        [Header("Managers")]
        [SerializeField] private SceneController sceneController;
        [SerializeField] private ColorThemeManager colorThemeManager;
        [SerializeField] private GridManager gridManager;

        private AppSettingsManager _appSettingsManager;
        private UserManager _userManager;

        #endregion

        #region MONO

        private void Awake()
        {
            InitializeManagers();
        }

        private void Start()
        {
            UpdateUIToDefault();
        }

        private void OnEnable()
        {
            RegisterEvents(true);
        }

        private void OnDisable()
        {
            RegisterEvents(false);
        }

        #endregion

        #region INITIALIZATION

        private void InitializeManagers()
        {
            _appSettingsManager = AppSettingsManager.Instance;
            _userManager = UserManager.Instance;
        }

        private void RegisterEvents(bool register)
        {
            if (register)
            {
                RegisterLevelButtonEvents(true);
                choosingButton.onClick.AddListener(FinishChooisingLevel);

                colorThemeManager.ChangingColorTheme += ImproveContinueGame;
            }
            else
            {
                RegisterLevelButtonEvents(false);
                choosingButton.onClick.RemoveListener(FinishChooisingLevel);

                colorThemeManager.ChangingColorTheme -= ImproveContinueGame;
            }
        }

        private void RegisterLevelButtonEvents(bool register)
        {
            foreach (var button in levelButtons)
                if (register)
                    button.onClick.AddListener(OnLevelButtonClicked);
                else
                    button.onClick.RemoveListener(OnLevelButtonClicked);
        }

        #endregion

        #region CORE LOGIC

        private void FinishChooisingLevel()
        {
            _appSettingsManager.SetIsNewGame(true);
            sceneController.LoadSudokuScene();
            gridManager?.StartNewGame();

            UpdateUIToDefault();
        }

        public void UpdateUIToDefault()
        {
            SelectLevelButton(_appSettingsManager.AppSettingData.DefaultLevel);
            ImproveContinueGame();
        }

        private void ImproveContinueGame()
        {
            if (continueGameButton != null && continueGameText != null)
            {
                bool hasUnfinishedSudoku = !_userManager.IsUnfinishedSudokuNull();
                float transparency = hasUnfinishedSudoku ? 1f : 0.5f;

                continueGameButton.SetTransparency(transparency);
                continueGameText.SetTransparency(transparency);

                continueGameButton.SetInteractable(hasUnfinishedSudoku);
            }
        }

        private void SelectLevelButton(int value) => levelButtons[value - 1].Select();

        #endregion

        #region CALLBACKS

        private void OnLevelButtonClicked()
        {
            Button button = levelButtons.FirstOrDefault(button => button.gameObject == EventSystem.current.currentSelectedGameObject);
            int level = levelButtons.ToList().IndexOf(button) + 1;

            _appSettingsManager.SetSelectedLevel(level);
        }

        #endregion
    }
}