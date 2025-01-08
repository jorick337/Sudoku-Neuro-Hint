using Unity.Mathematics;
using Help.Classes;
using UnityEngine;

namespace Game.Classes
{
    public class Sudoku
    {
        #region CONSTANTS

        private const int GRID_SIZE = 9; // Количество блоков судоку
        private const int BLOCK_SIZE = 9; // Количество ячеек в блоке
        private const int SIDE_LENGHT = 3; // Длина стороны блока

        #endregion

        #region CORE

        public int[,] MainGrid;
        public int[,] InitialGrid;
        public int[,] RealGrid;
        public Record Record;

        #endregion

        public Sudoku()
        {
            MainGrid = new int[GRID_SIZE, BLOCK_SIZE];
            InitialGrid = new int[GRID_SIZE, BLOCK_SIZE];
            RealGrid = new int[GRID_SIZE, BLOCK_SIZE];
            Record = null;
        }

        public Sudoku(int level)
        {
            Record = new(level, 0, 0, 0, 0);
            MainGrid = new int[GRID_SIZE, BLOCK_SIZE];

            GenerateNewGrid(MainGrid);

            RealGrid = (int[,])MainGrid.Clone();
            ApplyDifficulty(RealGrid, level);
            
            InitialGrid = (int[,])RealGrid.Clone();
        }

        #region CORE LOGIC

        private void GenerateNewGrid(int[,] grid)
        {
            FillDiagonalBlocks(grid);
            FillRemainingGrid(grid, 0, 0);
        }

        private void FillDiagonalBlocks(int[,] grid)
        {
            int step = (int)(math.sqrt(BLOCK_SIZE) + 1);

            for (int block = 0; block < BLOCK_SIZE; block += step)
                FillBlock(grid, block, 0);
        }

        private void FillBlock(int[,] grid, int block, int cell)
        {
            for (int i = cell; i < BLOCK_SIZE; i++)
            {
                int value;
                do
                    value = UnityEngine.Random.Range(1, BLOCK_SIZE + 1);
                while
                    (!grid.IsUnusedInBlock(block, value));

                grid[block, i] = value;
            }
        }

        private bool FillRemainingGrid(int[,] grid, int block, int cell)
        {
            if (block >= GRID_SIZE)
                return true;

            if (cell >= BLOCK_SIZE)
                return FillRemainingGrid(grid, block + 1, 0);

            // Пропустить диагональные блоки
            if (block % (SIDE_LENGHT + 1) == 0)
                return FillRemainingGrid(grid, block + 1, 0);

            for (int value = 1; value <= BLOCK_SIZE; value++)
            {
                if (grid.IsSafe(block, cell, value))
                {
                    grid[block, cell] = value;

                    if (FillRemainingGrid(grid, block, cell + 1))
                        return true;

                    grid[block, cell] = 0;
                }
            }

            return false;
        }

        #endregion

        #region DIFFICULTY

        private void ApplyDifficulty(int[,] grid, int level)
        {
            int cellsToClear = level switch
            {
                1 => UnityEngine.Random.Range(20, 25),
                2 => UnityEngine.Random.Range(26, 30),
                3 => UnityEngine.Random.Range(31, 35),
                4 => UnityEngine.Random.Range(36, 40),
                5 => UnityEngine.Random.Range(41, 45),
                6 => UnityEngine.Random.Range(46, 50),
                _ => 20
            };

            ClearCells(grid, cellsToClear);
        }

        private void ClearCells(int[,] grid, int cellsToClear)
        {
            while (cellsToClear > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, 81);
                int block = randomIndex / 9;
                int cell = randomIndex % 9;

                if (grid[block, cell] != 0)
                {
                    grid[block, cell] = 0;
                    cellsToClear -= 1;
                }
            }
        }

        #endregion

        #region SET

        public void SetValueRealGrid(Cell cell) => RealGrid[cell.Block, cell.Number] = cell.Value;
        public void SetRealGrid(int[,] grid) => RealGrid = (int[,])grid.Clone();

        #endregion

        #region BOOL

        public bool IsCellEqualMainGrid(Cell cell) => MainGrid[cell.Block, cell.Number] == cell.Value;
        public bool IsCellEqualRealGrid(Cell cell) => RealGrid[cell.Block, cell.Number] == cell.Value;

        #endregion
    }
}