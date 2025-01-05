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
            DefaultUsername = "DefaultUser";
            DefaultLevel = 1;
            DefaultSound = 0.5f;
            DifficultyLevels = new string[] { "Easy", "Medium", "Hard", "Very Hard", "Expert", "Master" };
            ScoreRecordPointsArray = new ScoreRecordPoints[]
            {
        new ScoreRecordPoints { FillCorrectly = 5, LevelFinished = 30, QuickFinish = 20, WrongFill = -3, HintTaken = -2, RevertMove = -1 },
        new ScoreRecordPoints { FillCorrectly = 10, LevelFinished = 50, QuickFinish = 30, WrongFill = -5, HintTaken = -3, RevertMove = -2 },
        new ScoreRecordPoints { FillCorrectly = 15, LevelFinished = 60, QuickFinish = 40, WrongFill = -6, HintTaken = -4, RevertMove = -3 },
        new ScoreRecordPoints { FillCorrectly = 20, LevelFinished = 80, QuickFinish = 50, WrongFill = -8, HintTaken = -5, RevertMove = -4 },
        new ScoreRecordPoints { FillCorrectly = 25, LevelFinished = 100, QuickFinish = 60, WrongFill = -10, HintTaken = -6, RevertMove = -5 },
        new ScoreRecordPoints { FillCorrectly = 30, LevelFinished = 120, QuickFinish = 70, WrongFill = -12, HintTaken = -7, RevertMove = -6 }
            };
            NameMainScene = "MainMenu";
            NameSudokuScene = "SudokuSolution";
            NameRecordsScene = "Records";
            ClassicColorTheme = new ColorTheme
            {
                Text = new Color(0, 0, 0, 1),
                Background = new Color(1, 1, 1, 1),
                HighlightedButton = new Color(0.9f, 0.9f, 0.9f, 1),
                PressedButton = new Color(0.8f, 0.8f, 0.8f, 1),
                SelectedButton = new Color(0.7f, 0.7f, 0.7f, 1),
                BackGroundButton = new Color(0.6f, 0.6f, 0.6f, 1),
                TextButton = new Color(0, 0, 0, 1),
                EffectColorOutline = new Color(0.5f, 0.5f, 0.5f, 1),
                BackGroundImageGrid = new Color(1, 1, 1, 1),
                SelectionImageGridBlock = new Color(0.9f, 0.9f, 0.9f, 1),
                SelectedImageGridCell = new Color(0.8f, 0.8f, 0.8f, 1),
                UnselectedImageGridCell = new Color(0.7f, 0.7f, 0.7f, 1),
                MainFocusedImageGridCell = new Color(1, 0.8f, 0.8f, 1),
                MinorFocusedImageGridCell = new Color(0.8f, 1, 0.8f, 1),
                BlockingImageGridCell = new Color(1, 1, 0.8f, 1),
                RightTextGridCellColor = new Color(0, 1, 0, 1),
                WrongTextGridCellColor = new Color(1, 0, 0, 1)
            };
            LightColorTheme = ClassicColorTheme; // Можно указать уникальные настройки, если нужно.
            DarkColorTheme = new ColorTheme
            {
                Text = new Color(1, 1, 1, 1),
                Background = new Color(0, 0, 0, 1),
                HighlightedButton = new Color(0.3f, 0.3f, 0.3f, 1),
                PressedButton = new Color(0.2f, 0.2f, 0.2f, 1),
                SelectedButton = new Color(0.1f, 0.1f, 0.1f, 1),
                BackGroundButton = new Color(0.1f, 0.1f, 0.1f, 1),
                TextButton = new Color(1, 1, 1, 1),
                EffectColorOutline = new Color(0.4f, 0.4f, 0.4f, 1),
                BackGroundImageGrid = new Color(0.2f, 0.2f, 0.2f, 1),
                SelectionImageGridBlock = new Color(0.3f, 0.3f, 0.3f, 1),
                SelectedImageGridCell = new Color(0.4f, 0.4f, 0.4f, 1),
                UnselectedImageGridCell = new Color(0.5f, 0.5f, 0.5f, 1),
                MainFocusedImageGridCell = new Color(1, 0.6f, 0.6f, 1),
                MinorFocusedImageGridCell = new Color(0.6f, 1, 0.6f, 1),
                BlockingImageGridCell = new Color(1, 1, 0.6f, 1),
                RightTextGridCellColor = new Color(0, 1, 0, 1),
                WrongTextGridCellColor = new Color(1, 0, 0, 1)
            };
            AutosaveSudoku = true;
            AutosaveRecord = true;
            UseHints = true;
        }
    }
}