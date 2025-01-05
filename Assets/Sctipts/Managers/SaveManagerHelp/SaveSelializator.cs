using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Game.Classes;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Game.Managers.Help
{
    public class SaveSerializator
    {
        #region APP DATA SETTINGS

        public JArray SerializeSettings(AppSettingData appSettingData)
        {
            JArray settingsJArray = new();
            var settingsObject = new JObject
            {
                { "DefaultUsername", appSettingData.DefaultUsername },
                { "DefaultLevel", appSettingData.DefaultLevel },
                { "DefaultSound", appSettingData.DefaultSound },
                { "DifficultyLevels", JArray.FromObject(appSettingData.DifficultyLevels) },
                { "ScoreRecordPointsArray", JArray.FromObject(
                    appSettingData.ScoreRecordPointsArray.Select(SerializeScoreRecordPoints)) },
                { "NameMainScene", appSettingData.NameMainScene },
                { "NameSudokuScene", appSettingData.NameSudokuScene },
                { "NameRecordsScene", appSettingData.NameRecordsScene },
                { "ClassicColorTheme", SerializeColorTheme(appSettingData.ClassicColorTheme) },
                { "LightColorTheme", SerializeColorTheme(appSettingData.LightColorTheme) },
                { "DarkColorTheme", SerializeColorTheme(appSettingData.DarkColorTheme) },
                { "AutosaveSudoku", appSettingData.AutosaveSudoku },
                { "AutosaveRecord", appSettingData.AutosaveRecord },
                { "UseHints", appSettingData.UseHints }
            };
            settingsJArray.Add(settingsObject);
            return settingsJArray;
        }

        private JObject SerializeScoreRecordPoints(ScoreRecordPoints scoreRecordPoints)
        {
            return new JObject
            {
                { "FillCorrectly", scoreRecordPoints.FillCorrectly },
                { "LevelFinished", scoreRecordPoints.LevelFinished },
                { "QuickFinish", scoreRecordPoints.QuickFinish },
                { "WrongFill", scoreRecordPoints.WrongFill },
                { "HintTaken", scoreRecordPoints.HintTaken },
                { "RevertMove", scoreRecordPoints.RevertMove }
            };
        }

        private JObject SerializeColorTheme(ColorTheme colorTheme)
        {
            return new JObject
            {
                { "Text", SerializeColor(colorTheme.Text) },
                { "Background", SerializeColor(colorTheme.Background) },
                { "HighlightedButton", SerializeColor(colorTheme.HighlightedButton) },
                { "PressedButton", SerializeColor(colorTheme.PressedButton) },
                { "SelectedButton", SerializeColor(colorTheme.SelectedButton) },
                { "BackGroundButton", SerializeColor(colorTheme.BackGroundButton) },
                { "TextButton", SerializeColor(colorTheme.TextButton) },
                { "EffectColorOutline", SerializeColor(colorTheme.EffectColorOutline) },
                { "BackGroundImageGrid", SerializeColor(colorTheme.BackGroundImageGrid) },
                { "SelectionImageGridBlock", SerializeColor(colorTheme.SelectionImageGridBlock) },
                { "SelectedImageGridCell", SerializeColor(colorTheme.SelectedImageGridCell) },
                { "UnselectedImageGridCell", SerializeColor(colorTheme.UnselectedImageGridCell) },
                { "MainFocusedImageGridCell", SerializeColor(colorTheme.MainFocusedImageGridCell) },
                { "MinorFocusedImageGridCell", SerializeColor(colorTheme.MinorFocusedImageGridCell) },
                { "BlockingImageGridCell", SerializeColor(colorTheme.BlockingImageGridCell) },
                { "RightTextGridCellColor", SerializeColor(colorTheme.RightTextGridCellColor) },
                { "WrongTextGridCellColor", SerializeColor(colorTheme.WrongTextGridCellColor) }
            };
        }

        private string SerializeColor(Color color)
        {
            string colorValues = string.Join(", ", new float[] { color.r, color.g, color.b, color.a }
                .Select(value => value.ToString("0.0", CultureInfo.InvariantCulture))); // Вместо запятых по умолчанию будут стоять точки
            
            return colorValues;
        }

        #endregion

        #region USERS

        public JArray SerializeUsers(List<User> users)
        {
            JArray usersJArray = new();
            foreach (var user in users)
            {
                var jObject = new JObject
                {
                    { "UserName", user.Username },
                    { "Records", JArray.FromObject(user.Records.Select(SerializeRecord)) },
                    { "UnfinishedSudoku", user.UnfinishedSudoku != null
                        ? SerializeSudoku(user.UnfinishedSudoku)
                        : null }
                };
                usersJArray.Add(jObject);
            }

            return usersJArray;
        }

        private JObject SerializeSudoku(Sudoku sudoku) => new()
        {
            { "MainGrid", ConvertGridToString(sudoku.MainGrid) },
            { "RealGrid", ConvertGridToString(sudoku.RealGrid) },
            { "InitialRealGrid", ConvertGridToString(sudoku.InitialGrid) },
            { "Record", SerializeRecord(sudoku.Record) }
        };

        private JObject SerializeRecord(Record record) => new()
        {
            { "Level", record.Level },
            { "NumberOfMistakes", record.NumberOfMistakes },
            { "NumberOfHints", record.NumberOfHints },
            { "TimeOfSolution", record.TimeOfSolution },
            { "Score", record.Score }
        };

        private string ConvertGridToString(int[,] grid) => string.Join(", ", grid.Cast<int>());

        #endregion
    }
}