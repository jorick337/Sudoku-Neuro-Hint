using Game.Classes;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Managers
{
    public class AppSettingsManager : MonoBehaviour
    {
        #region SINGLETON

        public static AppSettingsManager Instance { get; private set; }

        #endregion

        #region CORE

        public AppSettingData AppSettingData { get; private set; }

        public ScoreRecordPoints SelectedScoreRecordPoints { get; private set; }
        public ColorTheme SelectedColorTheme { get; private set; }

        public int SelectedLevel { get; private set; }
        
        public bool IsNewGame { get; private set; }

        [Header("Managers")]
        [SerializeField] private SaveManager saveManager;

        #endregion

        #region MONO

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
                
                InitializeValues();
            }
            else
                Destroy(gameObject);
        }

        private void OnApplicationQuit() 
        {
            saveManager.SaveAppSettingsData(AppSettingData);
        }

        #endregion

        #region INITIALIZATION

        private void InitializeValues()
        {
            AppSettingData = saveManager.LoadAppSettingsData();

            SetSelectedLevel(AppSettingData.DefaultLevel);
            SetIsNewGame(false);
            
            SetDefaultSound(AppSettingData.DefaultSound);

            SetSelectedColorTheme(AppSettingData.ClassicColorTheme);
        }

        #endregion

        #region COLOR THEME TYPE

        public enum ColorThemeType
        {
            Classic,
            Light,
            Dark
        }

        public ColorTheme GetAndSetColorTheme(ColorThemeType colorThemeType)
        {
            switch (colorThemeType)
            {
                case ColorThemeType.Classic:
                    SetSelectedColorTheme(AppSettingData.ClassicColorTheme);
                    return AppSettingData.ClassicColorTheme;
                case ColorThemeType.Light:
                    SetSelectedColorTheme(AppSettingData.LightColorTheme);
                    return AppSettingData.LightColorTheme;
                case ColorThemeType.Dark:
                    SetSelectedColorTheme(AppSettingData.DarkColorTheme);
                    return AppSettingData.DarkColorTheme;
            }

            return SelectedColorTheme;
        }

        #endregion

        #region SET

        public void SetSelectedScoreRecordPoints(int index) => SelectedScoreRecordPoints = AppSettingData.ScoreRecordPointsArray[index];

        public void SetSelectedLevel(int value) => SelectedLevel = math.clamp(value, 1, 6);
        public void SetIsNewGame(bool value) => IsNewGame = value;

        public void SetAutosaveSudoku(bool value) => AppSettingData.AutosaveSudoku = value;
        public void SetAutosaveRecord(bool value) => AppSettingData.AutosaveRecord = value;
        public void SetUseHints(bool value) => AppSettingData.UseHints = value;

        public void SetDefaultSound(float value) => AppSettingData.DefaultSound = value;

        private void SetSelectedColorTheme(ColorTheme value) => SelectedColorTheme = value;

        #endregion

        #region GET

        public string GetLevelStr(int value) => AppSettingData.DifficultyLevels[value - 1];

        #endregion
    }
}