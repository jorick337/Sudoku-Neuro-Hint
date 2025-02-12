using UnityEngine;
using System;
using Game.Classes;

namespace Game.Managers
{
    public class ColorThemeManager : MonoBehaviour
    {
        #region SINGLETON

        public static ColorThemeManager Instance { get; private set; }

        #endregion

        #region EVENTS

        public event Action ChangingColorTheme;
        public event Action SelectFocusCell;

        #endregion

        #region CORE

        [Header("Core")]
        [SerializeField] private bool updateColorsHere;

        [Header("Managers")]
        [SerializeField] private SceneController sceneController;

        private AppSettingsManager _appSettings;

        #endregion

        #region MONO

        private void Awake()
        {
            Instance = this;
            InitializeManagers();
        }

        private void Start()
        {
            if (updateColorsHere)
            {
                UpdateUIElementsAndColorTheme();
            }
        }

        #endregion

        #region INITIALIZATION

        private void InitializeManagers()
        {
            _appSettings = AppSettingsManager.Instance;
        }

        #endregion

        #region CORE LOGIC

        public void UpdateUIElementsAndColorTheme()
        {
            sceneController?.UpdateUIElements();
            ApplyColorTheme();
        }

        public void ApplyColorTheme(ColorTheme? colorTheme = null)
        {
            colorTheme ??= _appSettings.SelectedColorTheme;

            ApplyGeneralColors((ColorTheme)colorTheme);
            ApplyButtonColors((ColorTheme)colorTheme);
            ApplyOutlineColors((ColorTheme)colorTheme);
            ApplySudokuColors((ColorTheme)colorTheme);
            ApplyBlockersColors((ColorTheme)colorTheme);

            ChangingColorTheme?.Invoke();
            SelectFocusCell?.Invoke();
        }

        private void ApplyGeneralColors(ColorTheme colorTheme)
        {
            ApplyColorsToElements(sceneController.UITextElements, text => text.color = colorTheme.Text);
            ApplyColorsToElements(sceneController.UIImageElements, image => image.color = colorTheme.Background);
        }

        private void ApplyButtonColors(ColorTheme colorTheme)
        {
            ApplyColorsToElements(sceneController.Buttons, button =>
            {
                var colors = button.colors;
                colors.pressedColor = colorTheme.PressedButton;
                colors.selectedColor = colorTheme.SelectedButton;
                colors.highlightedColor = colorTheme.HighlightedButton;
                button.colors = colors;
            });

            ApplyColorsToElements(sceneController.ButtonTexts, text => text.color = colorTheme.TextButton);
            ApplyColorsToElements(sceneController.ButtonImages, image => image.color = colorTheme.BackGroundButton);
        }

        private void ApplyOutlineColors(ColorTheme colorTheme) =>
            ApplyColorsToElements(sceneController.Outlines, outline => outline.effectColor = colorTheme.EffectColorOutline);

        private void ApplyBlockersColors(ColorTheme colorTheme)
        {
            Color blockerColor = colorTheme.Background;
            blockerColor.a = 0.003f;
            ApplyColorsToElements(sceneController.Blockers, blocker => blocker.color = blockerColor);
        }

        private void ApplySudokuColors(ColorTheme colorTheme)
        {
            if (sceneController.SudokuGridBackground != null)
                sceneController.SudokuGridBackground.color =
                    colorTheme.BackGroundImageGrid;

            ApplyColorsToElements(sceneController.GridBlockHighlightImages, image => image.color = colorTheme.SelectionImageGridBlock);
        }

        private void ApplyColorsToElements<T>(T[] elements, Action<T> applyColor)
        {
            foreach (var element in elements)
                if (element != null)
                    applyColor(element);
        }

        #endregion

        #region CALLBACKS

        public void ApplyClassicTheme() => ApplyColorTheme(_appSettings.GetAndSetColorTheme(AppSettingsManager.ColorThemeType.Classic));
        public void ApplyLightTheme() => ApplyColorTheme(_appSettings.GetAndSetColorTheme(AppSettingsManager.ColorThemeType.Light));
        public void ApplyDarkTheme() => ApplyColorTheme(_appSettings.GetAndSetColorTheme(AppSettingsManager.ColorThemeType.Dark));

        #endregion
    }
}