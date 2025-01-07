using System.Text;
using Game.Classes;
using Help.Classes;
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

                    GridManager gridManager = GridManager.Instance;
                    gridManager.GridAdd.AddMove(gridManager, cellManager.Cell);
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

                cellManager.SetTextDirectly(GetNotepadText(cellManager.Cell));
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
            }

            cellManager.SetUpdateColors(UpdateColors);
            ColorThemeManager.Instance.ChangingColorTheme += UpdateColors;
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
            Sudoku sudoku =  GridManager.Instance.Sudoku;
            StringBuilder result = new();
            for (int i = 1; i <= 9; i++)
            {
                string value = cell.NotepadValues.Contains(i.ToString()) ? i.ToString() : "   ";
                string colorizedValue = sudoku.RealGrid.IsSafe(cell.Block, cell.Number, i)
                    ? $"<color=#{ColorUtility.ToHtmlStringRGBA(cell.CellColors.RightText)}>{value}</color>"
                    : $"<color=#{ColorUtility.ToHtmlStringRGBA(cell.CellColors.WrongText)}>{value}</color>";

                result.Append(colorizedValue);
                result.Append(i % 3 == 0 ? "\n" : "  ");
            }

            return result.ToString();
        }

        #endregion
    }
}