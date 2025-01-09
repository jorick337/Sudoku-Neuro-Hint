using System;
using System.Collections.Generic;
using System.Linq;
using Game.Classes;
using Unity.Mathematics;
using Game.Managers;
using Help.UI;

namespace Help.Classes
{
    public static class ClassesExtensions
    {
        #region SET

        public static void SetTextActivity(this CellManager[] cellManagers, bool isActive)
        {
            foreach (var cellManager in cellManagers)
            {
                cellManager.Text.SetEnabled(isActive);
            }
        }

        #endregion

        #region GET

        public static DetailedRecord[] GetAllDetailedRecords(this List<User> users)
        {
            return users
                .SelectMany(user => user.Records
                .Select(record => new DetailedRecord(user.Username, record)))
                .ToArray();
        }

        public static DetailedRecord[] GetSortedRecords<T>(
            this DetailedRecord[] detailedRecords,
            Func<DetailedRecord, T> keySelector,
            bool isDescending,
            int maximumCount)
        {
            return isDescending
                ? detailedRecords.OrderByDescending(keySelector).Take(maximumCount).ToArray()
                : detailedRecords.OrderBy(keySelector).Take(maximumCount).ToArray();
        }

        public static CellManager[] GetEmpty(this CellManager[] cellManagers)
        {
            return cellManagers
                .Where(cellManager => cellManager.Cell.Value == 0)
                .ToArray();
        }

        public static CellManager[] GetCellManagers(this GridBlock[] blocks,
        int initialBlock, int durationBlocks, int blockStep,
        int initialCell, int durationCells, int cellStep)
        {
            List<CellManager> result = new();
            for (int b = 0; b < durationBlocks / blockStep; b++)
                for (int c = 0; c < durationCells / cellStep; c++)
                {
                    CellManager[] block = blocks[initialBlock + b * blockStep].CellManagers;
                    CellManager correctCellManager = block[initialCell + c * cellStep];
                    result.Add(correctCellManager);
                }

            return result.ToArray();
        }

        #endregion

        #region BOOL

        public static bool IsSafe(this int[,] grid, int block, int cell, int value)
        {
            return grid.IsUnusedInBlock(block, value) &&
                    grid.IsUnusedInLineX(block, cell, value) &&
                    grid.IsUnusedInLineY(block, cell, value);
        }

        public static bool IsUnusedInBlock(this int[,] grid, int block, int value)
        {
            int gridSize = grid.GetLength(0);
            for (int cell = 0; cell < gridSize; cell++)
                if (grid[block, cell] == value)
                    return false;

            return true;
        }

        public static bool IsUnusedInLineX(this int[,] grid, int block, int cell, int value)
        {
            int gridSize = grid.GetLength(0);
            int sideLenght = (int)math.sqrt(gridSize);

            int startBlock = block / sideLenght * sideLenght;
            int startCell = cell / sideLenght * sideLenght;

            for (int b = startBlock; b < startBlock + sideLenght; b++)
                for (int c = startCell; c < startCell + sideLenght; c++)
                    if (grid[b, c] == value)
                        return false;

            return true;
        }

        public static bool IsUnusedInLineY(this int[,] grid, int block, int cell, int value)
        {
            int gridSize = grid.GetLength(0);
            int sideLenght = (int)math.sqrt(gridSize);

            int startBlock = block % sideLenght;
            int startCell = cell % sideLenght;

            for (int b = startBlock; b < gridSize; b += sideLenght)
                for (int c = startCell; c < gridSize; c += sideLenght)
                    if (grid[b, c] == value)
                        return false;

            return true;
        }

        #endregion
    }
}