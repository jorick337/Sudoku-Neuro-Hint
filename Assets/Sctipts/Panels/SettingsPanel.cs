using UnityEngine;
using UnityEngine.UI;
using Game.Managers;

namespace Game.Panels
{
    public class AppSettingsPanel : MonoBehaviour
    {
        #region CORE

        [Header("Music")]
        [SerializeField] private Slider GeneralSoundSlider;

        [Header("Color Themes")]
        [SerializeField] private Button classicColorThemeButton;
        [SerializeField] private Button lightColorThemeButton;
        [SerializeField] private Button darkColorThemeButton;

        [Header("Autosave")]
        [SerializeField] private Toggle autosaveRecordToggle;
        [SerializeField] private Toggle autosaveSudokuToggle;

        [Header("Hints")]
        [SerializeField] private Toggle hintsToggle;

        [Header("Managers")]
        [SerializeField] private ColorThemeManager colorThemeManager;

        private AppSettingsManager _appSettings;
        private SoundManager _soundManager;

        #endregion

        #region MONO

        private void Start()
        {
            InitializeManagers();
            InitializeValues();
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
            _appSettings = AppSettingsManager.Instance;
            _soundManager = SoundManager.Instance;
        }

        private void InitializeValues()
        {
            GeneralSoundSlider.value = _appSettings.AppSettingData.DefaultSound;

            autosaveSudokuToggle.isOn = _appSettings.AppSettingData.AutosaveSudoku;
            autosaveRecordToggle.isOn = _appSettings.AppSettingData.AutosaveRecord;
            hintsToggle.isOn = _appSettings.AppSettingData.UseHints;
        }

        private void RegisterEvents(bool register)
        {
            if (register)
            {
                GeneralSoundSlider.onValueChanged.AddListener(_soundManager.UpdateGeneralSound);

                classicColorThemeButton.onClick.AddListener(colorThemeManager.ApplyClassicTheme);
                lightColorThemeButton.onClick.AddListener(colorThemeManager.ApplyLightTheme);
                darkColorThemeButton.onClick.AddListener(colorThemeManager.ApplyDarkTheme);

                autosaveSudokuToggle.onValueChanged.AddListener(_appSettings.SetAutosaveSudoku);
                autosaveRecordToggle.onValueChanged.AddListener(_appSettings.SetAutosaveRecord);

                hintsToggle.onValueChanged.AddListener(_appSettings.SetUseHints);
            }
            else
            {
                GeneralSoundSlider.onValueChanged.RemoveListener(_soundManager.UpdateGeneralSound);

                classicColorThemeButton.onClick.RemoveListener(colorThemeManager.ApplyClassicTheme);
                lightColorThemeButton.onClick.RemoveListener(colorThemeManager.ApplyLightTheme);
                darkColorThemeButton.onClick.RemoveListener(colorThemeManager.ApplyDarkTheme);

                autosaveSudokuToggle.onValueChanged.RemoveListener(_appSettings.SetAutosaveSudoku);
                autosaveRecordToggle.onValueChanged.RemoveListener(_appSettings.SetAutosaveRecord);

                hintsToggle.onValueChanged.RemoveListener(_appSettings.SetUseHints);
            }
        }

        #endregion
    }
}