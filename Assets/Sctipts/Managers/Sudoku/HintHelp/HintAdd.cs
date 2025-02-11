using System.Linq;
using Game.Classes;
using UnityEngine;

namespace Game.Managers.Help
{
    public class HintAdd
    {
        #region CONSTANTS

        private const string VALUE_PLACEHOLDER = "0";

        private const string TEXT_CELLS = "эти ячейки";
        private const string TEXT_SELECTED_AREAS = "выделенные области";
        private const string TEXT_BLOCK = "этом блоке";
        private const string TEXT_CELL = "ячейка";

        private const string TEXT_THIS_CELL = "эту ячейку";

        #endregion

        #region TEXTS

        public void AddValuesToExplanations(HintManager hintManager)
        {
            Hint hint = hintManager.Hint;
            string value = hint.Value.ToString();

            if (hint.View == HintManager.VIEW_ONE)
            {
                hintManager.SetTextExplanation(1, VALUE_PLACEHOLDER, value);
                hintManager.SetTextExplanation(2, VALUE_PLACEHOLDER, value);
            }
            else if (hint.View == HintManager.VIEW_TWO)
            {
                string valuesStr = BuildValuesString(hint.NonRepeatingCellManagers);

                hintManager.SetTextExplanation(1, VALUE_PLACEHOLDER, valuesStr);
                hintManager.SetTextExplanation(2, VALUE_PLACEHOLDER, value);
            }
        }

        private string BuildValuesString(CellManager[] cellManagers)
        {
            IOrderedEnumerable<int> values =
                cellManagers
                .Select(cellManager => cellManager.Cell.Value)
                .OrderBy(value => value);

            return string.Join(", ", values.Take(values.Count() - 1)) + " и " + values.Last();
        }

        #endregion

        #region COLORS

        public void AddColorsToExplanations(HintManager hintManager)
        {
            if (hintManager.Hint == null)
            {
                return;
            }

            ColorTheme colorTheme = AppSettingsManager.Instance.SelectedColorTheme;
            string mainFocusedColor = GetColorHex(colorTheme.MainFocusedImageGridCell);
            string minorFocusedColor = GetColorHex(colorTheme.MinorFocusedImageGridCell);
            string selectedCellColor = GetColorHex(colorTheme.SelectedImageGridCell);
            string selectedBlockColor = GetColorHex(colorTheme.SelectionImageGridBlock);

            var replacements = hintManager.Hint.View switch // Смена цветов в тексте объяснений
            {
                HintManager.VIEW_ONE => new (int index, string oldValue, string newValue)[]
                {
                    (0, TEXT_CELLS, $"<b><color=#{minorFocusedColor}>{TEXT_CELLS}</color></b>"),
                    (0, TEXT_SELECTED_AREAS, $"<b><color=#{selectedCellColor}>{TEXT_SELECTED_AREAS}</color></b>"),
                    (1, TEXT_BLOCK, $"<b><color=#{selectedBlockColor}>{TEXT_BLOCK}</color></b>"),
                    (1, TEXT_CELL, $"<b><color=#{mainFocusedColor}>{TEXT_CELL}</color></b>")
                },
                HintManager.VIEW_TWO => new (int index, string oldValue, string newValue)[]
                {
                    (0, TEXT_THIS_CELL, $"<b><color=#{selectedCellColor}>{TEXT_THIS_CELL}</color></b>")
                },
                _ => null
            };

            foreach (var (index, oldValue, newValue) in replacements)
                hintManager.SetTextExplanation(index, oldValue, newValue);

            HighlightNumbers(hintManager, mainFocusedColor, minorFocusedColor);
        }

        private void HighlightNumbers(HintManager hintManager, string mainFocusedColor, string minorFocusedColor)
        {
            int valueHint = hintManager.Hint.Value;
            if (hintManager.Hint.View == HintManager.VIEW_TWO)
            {
                for (int i = 1; i <= 9; i++)
                {
                    hintManager.SetTextExplanation(1, $" {i}", $" <color=#{minorFocusedColor}>{i}</color>");
                }
            }
            else 
            {
                hintManager.SetTextExplanation(1, $" {valueHint}", $" <color=#{mainFocusedColor}>{valueHint}</color>");
            }

            hintManager.SetTextExplanation(2, $" {valueHint}", $" <color=#{mainFocusedColor}>{valueHint}</color>");
        }

        #endregion

        #region GET

        private string GetColorHex(Color color) => ColorUtility.ToHtmlStringRGBA(color);

        #endregion
    }
}