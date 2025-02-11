using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Game.Classes;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Game.Managers.Help
{
    public class SaveDeserializator
    {
        #region APPSETTINGDATA

        public AppSettingData DeserializeSettings(JArray settingsJArray)
        {
            JToken settingsToken = settingsJArray[0];
            AppSettingData appSettingData = new()
            {
                DefaultUsername = settingsToken["DefaultUsername"]?.ToString(),
                DefaultLevel = settingsToken["DefaultLevel"] != null ? (int)settingsToken["DefaultLevel"] : 0,
                DefaultSound = settingsToken["DefaultSound"] != null ? (float)settingsToken["DefaultSound"] : 0f,
                DifficultyLevels = settingsToken["DifficultyLevels"]?.ToObject<string[]>(),
                ScoreRecordPointsArray = settingsToken["ScoreRecordPointsArray"]?.ToObject<ScoreRecordPoints[]>(),
                NameMainScene = settingsToken["NameMainScene"]?.ToString(),
                NameSudokuScene = settingsToken["NameSudokuScene"]?.ToString(),
                NameRecordsScene = settingsToken["NameRecordsScene"]?.ToString(),
                ClassicColorTheme = DeserializeColorTheme(settingsToken["ClassicColorTheme"]),
                LightColorTheme = DeserializeColorTheme(settingsToken["LightColorTheme"]),
                DarkColorTheme = DeserializeColorTheme(settingsToken["DarkColorTheme"]),
                AutosaveSudoku = settingsToken["AutosaveSudoku"] != null && (bool)settingsToken["AutosaveSudoku"],
                AutosaveRecord = settingsToken["AutosaveRecord"] != null && (bool)settingsToken["AutosaveRecord"],
                UseHints = settingsToken["UseHints"] != null && (bool)settingsToken["UseHints"]
            };

            return appSettingData;
        }

        private ColorTheme DeserializeColorTheme(JToken colorThemeToken)
        {
            return new ColorTheme
            {
                Text = DeserializeColor(colorThemeToken["Text"].ToString()),
                Background = DeserializeColor(colorThemeToken["Background"].ToString()),
                HighlightedButton = DeserializeColor(colorThemeToken["HighlightedButton"].ToString()),
                PressedButton = DeserializeColor(colorThemeToken["PressedButton"].ToString()),
                SelectedButton = DeserializeColor(colorThemeToken["SelectedButton"].ToString()),
                BackGroundButton = DeserializeColor(colorThemeToken["BackGroundButton"].ToString()),
                TextButton = DeserializeColor(colorThemeToken["TextButton"].ToString()),
                EffectColorOutline = DeserializeColor(colorThemeToken["EffectColorOutline"].ToString()),
                BackGroundImageGrid = DeserializeColor(colorThemeToken["BackGroundImageGrid"].ToString()),
                SelectionImageGridBlock = DeserializeColor(colorThemeToken["SelectionImageGridBlock"].ToString()),
                SelectedImageGridCell = DeserializeColor(colorThemeToken["SelectedImageGridCell"].ToString()),
                UnselectedImageGridCell = DeserializeColor(colorThemeToken["UnselectedImageGridCell"].ToString()),
                MainFocusedImageGridCell = DeserializeColor(colorThemeToken["MainFocusedImageGridCell"].ToString()),
                MinorFocusedImageGridCell = DeserializeColor(colorThemeToken["MinorFocusedImageGridCell"].ToString()),
                BlockingImageGridCell = DeserializeColor(colorThemeToken["BlockingImageGridCell"].ToString()),
                RightTextGridCellColor = DeserializeColor(colorThemeToken["RightTextGridCellColor"].ToString()),
                WrongTextGridCellColor = DeserializeColor(colorThemeToken["WrongTextGridCellColor"].ToString())
            };
        }

        private Color DeserializeColor(string colorString)
        {
            float[] valuesOfColor = colorString.Split(',').Select(x => float.Parse(x.Trim(), CultureInfo.InvariantCulture)).ToArray();
            return new Color
            {
                r = valuesOfColor[0],
                g = valuesOfColor[1],
                b = valuesOfColor[2],
                a = valuesOfColor[3]
            };
        }

        #endregion

        #region USERS

        public List<User> DeserializeUsers(JArray usersJArray)
        {
            List<User> users = new();
            foreach (var userToken in usersJArray)
            {
                User user = new()
                {
                    Username = userToken["UserName"].ToString(),
                    Records = DeserializeRecords(userToken["Records"]),
                    UnfinishedSudoku = userToken["UnfinishedSudoku"] != null
                        && userToken["UnfinishedSudoku"].Type != JTokenType.Null
                        ? DeserializeSudoku(userToken["UnfinishedSudoku"])
                        : null
                };
                users.Add(user);
            }

            return users;
        }

        private Sudoku DeserializeSudoku(JToken sudokuToken)
        {
            return new()
            {
                MainGrid = ConvertLineToGrid(sudokuToken["MainGrid"]?.ToString() ?? ""),
                RealGrid = ConvertLineToGrid(sudokuToken["RealGrid"]?.ToString() ?? ""),
                InitialGrid = ConvertLineToGrid(sudokuToken["InitialGrid"]?.ToString() ?? ""),
                Record = sudokuToken["Record"]?.ToObject<Record>() ?? null
            };
        }

        private List<Record> DeserializeRecords(JToken recordsToken)
        {
            List<Record> records = new();

            foreach (var recordToken in recordsToken)
                records.Add(new(
                    recordToken["Level"].ToObject<int>(),
                    recordToken["NumberOfHints"].ToObject<int>(),
                    recordToken["NumberOfMistakes"].ToObject<int>(),
                    recordToken["TimeOfSolution"].ToObject<float>(),
                    recordToken["Score"].ToObject<int>()));

            return records;
        }

        private int[,] ConvertLineToGrid(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return new int[0, 0];

            string[] elements = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int size = (int)Math.Sqrt(elements.Length);

            int[,] grid = new int[size, 9];
            for (int row = 0; row < size; row++)
                for (int col = 0; col < 9; col++)
                    grid[row, col] = int.Parse(elements[row * size + col].Trim());

            return grid;
        }

        #endregion
    }
}