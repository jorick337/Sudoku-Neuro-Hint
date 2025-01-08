using Help.UI;
using static Game.Managers.Help.CellHightlighter;

namespace Game.Managers.Help
{
    public class CellAdd
    {
        #region CORE LOGIC

        public void AddValueWithoutMoveAndChecks(CellManager cellManager, int value)
        {
            cellManager.Cell.SetValue(value);

            if (GridManager.Instance.GridBlocks.IsNotepadeMode && cellManager.InputField.characterLimit == 0) // Если включен NotepadMode
                cellManager.CellUI.SwitchToNormalMode(cellManager);

            cellManager.InputField.SetReadOnly(false);
            UpdateCellState(cellManager, true);
            cellManager.SetTextDirectly(cellManager.Cell.Value.ToString());
        }

        #endregion

        #region UPDATE

        public void UpdateCellState(CellManager cellManager, bool undoLastMove)
        {
            GridManager gridManager = GridManager.Instance;

            gridManager.GridBlocks.SetFocusedCellManager(cellManager);
            gridManager.Sudoku.SetValueRealGrid(cellManager.Cell);

            cellManager.CellHightlighter.Select(cellManager);
            UpdateCellHighlighting(cellManager, undoLastMove);

            gridManager.CheckGameCompletion();
        }

        private void UpdateCellHighlighting(CellManager cellManager, bool undoLastMove)
        {
            GridManager gridManager = GridManager.Instance;
            CellHightlighter cellHightlighter = cellManager.CellHightlighter;

            if (gridManager.Sudoku.IsCellEqualMainGrid(cellManager.Cell))
            {
                cellHightlighter.HighlightCell(cellManager, CellHighlightType.Right);
                cellManager.InputField.SetReadOnly(true);

                if (!cellManager.Cell.AddScoreForCorrectFilling && !undoLastMove) // Решить правильно можно только один раз
                {
                    cellManager.Cell.SetAddScoreForCorrectFilling(true);
                    gridManager.GridAdd.AddScoreByScoreType(gridManager, GridAdd.ScoreType.FillCorrectly);
                }
            }
            else 
            {
                cellHightlighter.HighlightCell(cellManager, CellHighlightType.Wrong);
                if (!undoLastMove) // Только если это не возвращение значения
                {
                    gridManager.GridAdd.AddMistake(gridManager);
                }
            }
        }

        #endregion
    }
}