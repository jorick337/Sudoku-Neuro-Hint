using System.Collections.Generic;
using System.Linq;
using Game.Managers;
using Help.Classes;
using UnityEngine;

namespace Game.Classes
{
    public class Hint
    {
        #region CONSTANTS

        public const string VIEW_ONE = "SingleCandidate";
        public const string VIEW_TWO = "SinglePosition";

        #endregion

        #region CORE

        public CellManager CellManager { get; private set; }
        public GridBlock GridBlock { get; private set; }
        public int Value { get; private set; }
        public string View { get; private set; }

        public CellManager[] NonRepeatingCellManagers { get; private set; }
        public bool? HighlightLineX { get; private set; }
        public bool? HighlightLineY { get; private set; }

        #endregion

        #region CONSTRUCTORS

        public Hint()
        {
            GridManager gridManager = GridManager.Instance;
            Find(gridManager, gridManager.GridBlocks);
        }

        #endregion

        #region CORE LOGIC

        private void Find(GridManager sudokuManager, GridBlocks gridBlocks)
        {
            foreach (var gridBlock in gridBlocks.Blocks)
            {
                TrySingleCandidate(sudokuManager, gridBlock);
                if (CellManager == null)
                {
                    TrySinglePosition(gridBlock);
                }
            }
        }

        private void TrySingleCandidate(GridManager gridManager, GridBlock gridBlock)
        {
            CellManager[] emptyCellManagers = gridBlock.CellManagers.Where(cellManager => cellManager.Cell.Value == 0).ToArray();

            for (int value = 1; value <= 9; value++)
            {
                CellManager[] correctCellManagers = emptyCellManagers // Беру только те пустные значения, которых безопастны по RealGrid
                .Where(cellManager =>
                    {
                        Cell cell = cellManager.Cell;
                        return gridManager.Sudoku.RealGrid.IsSafe(cell.Block, cell.Number, value);
                    }
                )
                .ToArray();

                if (correctCellManagers.Length == 1) // Если оно только одно то это подсказка
                {
                    CellManager = correctCellManagers[0];
                    GridBlock = gridBlock;
                    Value = value;
                    View = VIEW_ONE;
                }
            }
        }

        private void TrySinglePosition(GridBlock gridBlock)
        {
            CellManager[] emptyCellManagers = gridBlock.CellManagers.Where(cellManager => cellManager.Cell.Value == 0).ToArray();
            CellManager[] nonRepeatingCellManagersBlock = GetNonRepeatingCellManagers(gridBlock.CellManagers);

            foreach (CellManager emptyCellManager in emptyCellManagers)
            {
                CellGroups cellGroups = emptyCellManager.Cell.CellGroups;

                CellManager[] nonRepeatingCellManagersLineX = GetNonRepeatingCellManagers(cellGroups.LineX);
                CellManager[] nonRepeatingCellManagersLineY = GetNonRepeatingCellManagers(cellGroups.LineY);

                CellManager[] allNonRepeatingCellManagers = nonRepeatingCellManagersBlock
                .Concat(nonRepeatingCellManagersLineX)
                .Concat(nonRepeatingCellManagersLineY)
                .ToArray();
                allNonRepeatingCellManagers = GetNonRepeatingCellManagers(allNonRepeatingCellManagers);

                if (allNonRepeatingCellManagers.Length == 8)
                {
                    int value = Enumerable.Range(1, 9)
                    .Except(allNonRepeatingCellManagers.Select(cellManager => cellManager.Cell.Value))
                    .FirstOrDefault();

                    Cell cell = emptyCellManager.Cell;
                    bool highlightLineX = allNonRepeatingCellManagers.Any(cellManager => cellManager.Cell.Block % 3 != cell.Block % 3);
                    bool highlightLineY = allNonRepeatingCellManagers.Any(cellManager => cellManager.Cell.Block / 3 != cell.Block / 3);

                    CellManager = emptyCellManager;
                    GridBlock = gridBlock;
                    Value = value;
                    View = VIEW_TWO;
                    NonRepeatingCellManagers = allNonRepeatingCellManagers;
                    HighlightLineX = highlightLineX;
                    HighlightLineY = highlightLineY;

                    return;
                }
            }
        }

        #endregion

        #region GET

        private CellManager[] GetNonRepeatingCellManagers(CellManager[] cellManagers)
        {
            List<CellManager> nonRepeatingCellManagers = new();
            HashSet<int> values = new();

            foreach (CellManager cellManager in cellManagers)
            {
                int value = cellManager.Cell.Value;

                if (value != 0 && !values.Contains(value))
                {
                    values.Add(value);
                    nonRepeatingCellManagers.Add(cellManager);
                }
            }

            return nonRepeatingCellManagers.ToArray();
        }

        #endregion
    }
}