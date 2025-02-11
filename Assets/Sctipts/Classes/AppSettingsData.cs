using UnityEngine;

namespace Game.Classes
{
    [System.Serializable]
    public class AppSettingData
    {
        public string DefaultUsername;
        public int DefaultLevel;
        public float DefaultSound;

        public string[] DifficultyLevels;
        public ScoreRecordPoints[] ScoreRecordPointsArray;

        public string NameMainScene;
        public string NameSudokuScene;
        public string NameRecordsScene;

        public ColorTheme ClassicColorTheme;
        public ColorTheme LightColorTheme;
        public ColorTheme DarkColorTheme;

        public bool AutosaveSudoku;
        public bool AutosaveRecord;
        public bool UseHints;

        public AppSettingData()
        {
            DefaultUsername = "Sudoku";
            DefaultLevel = 1;
            DefaultSound = 0.5f;
            
            DifficultyLevels = new string[] { "Легкий", "Средний", "Сложный", "Очень сложный", "Эксперт", "Мастер" };
            ScoreRecordPointsArray = new ScoreRecordPoints[]
            {
                new() { FillCorrectly = 5, LevelFinished = 30, QuickFinish = 20, WrongFill = -3, HintTaken = -2, RevertMove = -1 },
                new() { FillCorrectly = 10, LevelFinished = 50, QuickFinish = 30, WrongFill = -5, HintTaken = -3, RevertMove = -2 },
                new() { FillCorrectly = 15, LevelFinished = 60, QuickFinish = 40, WrongFill = -6, HintTaken = -4, RevertMove = -3 },
                new() { FillCorrectly = 20, LevelFinished = 80, QuickFinish = 50, WrongFill = -8, HintTaken = -5, RevertMove = -4 },
                new() { FillCorrectly = 25, LevelFinished = 100, QuickFinish = 60, WrongFill = -10, HintTaken = -6, RevertMove = -5 },
                new() { FillCorrectly = 30, LevelFinished = 120, QuickFinish = 70, WrongFill = -12, HintTaken = -7, RevertMove = -6 }
            };

            NameMainScene = "Menu";
            NameSudokuScene = "Sudoku";
            NameRecordsScene = "Records";

            ClassicColorTheme = new ColorTheme
            {
                Text = new Color(0.2f, 0.2f, 0.2f, 1.0f),
                Background = new Color(1.0f, 0.9f, 0.8f, 1.0f),
                HighlightedButton = new Color(0.9f, 0.8f, 0.6f, 1.0f),
                PressedButton = new Color(0.9f, 0.8f, 0.6f, 1.0f),
                SelectedButton = new Color(0.8f, 0.7f, 0.5f, 1.0f),
                BackGroundButton = new Color(0.8f, 0.7f, 0.5f, 1.0f),
                TextButton = new Color(0.2f, 0.2f, 0.2f, 0.9f),
                EffectColorOutline = new Color(0.6f, 0.5f, 0.3f, 0.2f),
                BackGroundImageGrid = new Color(0.8f, 0.7f, 0.6f, 1.0f),
                SelectionImageGridBlock = new Color(0.2f, 0.3f, 0.7f, 1.0f),
                SelectedImageGridCell = new Color(0.9f, 0.8f, 0.6f, 1.0f),
                UnselectedImageGridCell = new Color(0.7f, 0.6f, 0.4f, 1.0f),
                MainFocusedImageGridCell = new Color(0.6f, 0.5f, 0.2f, 1.0f),
                MinorFocusedImageGridCell = new Color(0.6f, 0.5f, 0.3f, 1.0f),
                BlockingImageGridCell = new Color(0.7f, 0.6f, 0.4f, 1.0f),
                RightTextGridCellColor = new Color(0.2f, 0.2f, 0.2f, 1.0f),
                WrongTextGridCellColor = new Color(0.8f, 0.2f, 0.2f, 1.0f)
            };
            LightColorTheme = new ColorTheme
            {
                Text = new Color(0.1f, 0.1f, 0.1f, 0.8f),
                Background = new Color(1.0f, 1.0f, 1.0f, 1.0f),
                HighlightedButton = new Color(0.9f, 0.9f, 0.9f, 1.0f),
                PressedButton = new Color(0.8f, 0.8f, 0.8f, 1.0f),
                SelectedButton = new Color(0.7f, 0.7f, 0.7f, 1.0f),
                BackGroundButton = new Color(0.5f, 0.5f, 0.5f, 1.0f),
                TextButton = new Color(0.1f, 0.1f, 0.1f, 0.9f),
                EffectColorOutline = new Color(0.5f, 0.5f, 0.5f, 0.2f),
                BackGroundImageGrid = new Color(0.3f, 0.3f, 0.3f, 1.0f),
                SelectionImageGridBlock = new Color(0.8f, 0.8f, 0.8f, 1.0f),
                SelectedImageGridCell = new Color(0.7f, 0.7f, 0.7f, 1.0f),
                UnselectedImageGridCell = new Color(0.6f, 0.6f, 0.6f, 1.0f),
                MainFocusedImageGridCell = new Color(0.9f, 0.5f, 0.5f, 1.0f),
                MinorFocusedImageGridCell = new Color(0.8f, 0.9f, 0.8f, 1.0f),
                BlockingImageGridCell = new Color(0.6f, 0.6f, 0.6f, 1.0f),
                RightTextGridCellColor = new Color(0.2f, 0.2f, 0.2f, 1.0f),
                WrongTextGridCellColor = new Color(0.9f, 0.1f, 0.1f, 1.0f)
            };
            DarkColorTheme = new ColorTheme
            {
                Text = new Color(0.9f, 0.9f, 0.9f, 1.0f),
                Background = new Color(0.1f, 0.1f, 0.1f, 1.0f),
                HighlightedButton = new Color(0.4f, 0.4f, 0.4f, 1.0f),
                PressedButton = new Color(0.3f, 0.3f, 0.3f, 1.0f),
                SelectedButton = new Color(0.2f, 0.2f, 0.2f, 1.0f),
                BackGroundButton = new Color(0.3f, 0.3f, 0.3f, 1.0f),
                TextButton = new Color(0.9f, 0.9f, 0.9f, 0.9f),
                EffectColorOutline = new Color(0.5f, 0.5f, 0.5f, 0.3f),
                BackGroundImageGrid = new Color(0.3f, 0.3f, 0.3f, 1.0f),
                SelectionImageGridBlock = new Color(0.4f, 0.4f, 0.4f, 1.0f),
                SelectedImageGridCell = new Color(0.4f, 0.4f, 0.4f, 1.0f),
                UnselectedImageGridCell = new Color(0.6f, 0.6f, 0.6f, 1.0f),
                MainFocusedImageGridCell = new Color(0.8f, 0.4f, 0.4f, 1.0f),
                MinorFocusedImageGridCell = new Color(0.4f, 0.8f, 0.4f, 0.6f),
                BlockingImageGridCell = new Color(0.6f, 0.6f, 0.6f, 1.0f),
                RightTextGridCellColor = new Color(0.9f, 0.9f, 0.9f, 1.0f),
                WrongTextGridCellColor = new Color(0.9f, 0.2f, 0.2f, 1.0f)
            };

            AutosaveSudoku = true;
            AutosaveRecord = true;
            UseHints = true;
        }
    }
}