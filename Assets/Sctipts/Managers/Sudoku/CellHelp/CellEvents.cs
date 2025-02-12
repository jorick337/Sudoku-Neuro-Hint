using System.Diagnostics;
using System.Linq;
using System.Text;
using Game.Classes;
using Help.Classes;
using Help.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Managers.Help
{
    public class CellEvents
    {
        #region ON VALUE CHANGED

        public void RegisterOnValueChangedForNormalMode(CellManager cellManager)
        {
            void onValueChanged(string arg0)
            {
                if (!cellManager.InputField.readOnly)
                {
                    int value = GetValidatedLastValue(cellManager.InputField);

                    cellManager.Cell.SetValue(value);
                    cellManager.SetTextDirectly(value.ToString());

                    cellManager.CellAdd.UpdateCellState(cellManager, false);
                    UpdateNotepadValuesColorsForAll(cellManager);
                }
            }

            cellManager.SetOnValueChanged(onValueChanged);
        }

        public void RegisterOnValueChangedForNotepadMode(CellManager cellManager)
        {
            void onValueChanged(string arg0)
            {
                string value = GetValidatedLastValue(cellManager.InputField).ToString();
                if (cellManager.Cell.NotepadValues.Contains(value))
                {
                    cellManager.Cell.NotepadValues.Remove(value);
                }
                else
                {
                    cellManager.Cell.NotepadValues.Add(value);
                }

                UpdateNotepadValuesColorsForAll(cellManager);
            }

            cellManager.SetOnValueChanged(onValueChanged);
        }

        #endregion

        #region UPDATE COLORS

        public void RigisterUpdateColors(CellManager cellManager)
        {
            void UpdateColors()
            {
                cellManager.Cell.SetCellColors(new(AppSettingsManager.Instance.SelectedColorTheme));
                cellManager.CellHightlighter.HighlightCell(cellManager, CellHightlighter.CellHighlightType.Unselected);
                UpdateNotepadValuesColor(cellManager);

                if (cellManager.InputField.readOnly)
                {
                    cellManager.Text.SetColor(cellManager.Cell.CellColors.RightText);
                }
                else
                {
                    cellManager.Text.SetColor(cellManager.Cell.CellColors.WrongText);
                }
            }

            cellManager.SetUpdateColors(UpdateColors);
            ColorThemeManager.Instance.ChangingColorTheme += UpdateColors;
        }

        public void UpdateNotepadValuesColorsForAll(CellManager cellManager)
        {
            CellManager[] cellManagers = cellManager.Cell.CellGroups.Block
                .Concat(cellManager.Cell.CellGroups.LineX)
                .Concat(cellManager.Cell.CellGroups.LineY)
                .ToArray();

            foreach (var cell in cellManagers)
            {
                UpdateNotepadValuesColor(cell);
            }
        }

        private void UpdateNotepadValuesColor(CellManager cellManager)
        {
            if (cellManager.Cell.NotepadValues.Count > 0 && cellManager.Cell.Value == 0)    // Проверка на наличие заметок
            {
                cellManager.SetTextDirectly(GetNotepadText(cellManager.Cell));
            }
        }

        #endregion

        #region GET

        public int GetValidatedLastValue(InputField inputField)
        {
            if (!string.IsNullOrEmpty(inputField.text) && !inputField.readOnly)
            {
                char lastChar = inputField.text[^1];
                if (lastChar >= '1' && lastChar <= '9')
                    return lastChar - '0';
                else
                    return 0;
            }
            return 0;
        }

        public string GetNotepadText(Cell cell)
        {
            Sudoku sudoku = GridManager.Instance.Sudoku;
            StringBuilder result = new();
            for (int i = 1; i <= 9; i++)
            {
                string value = cell.NotepadValues.Contains(i.ToString()) ? i.ToString() : "   ";
                string colorizedValue = sudoku.RealGrid.IsSafe(cell.Block, cell.Number, i)
                    ? $"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGBA(cell.CellColors.RightText)}>{value}</color>"
                    : $"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGBA(cell.CellColors.WrongText)}>{value}</color>";

                result.Append(colorizedValue);
                result.Append(i % 3 == 0 ? "\n" : "  ");
            }

            return result.ToString();
        }

        #endregion
    }
}