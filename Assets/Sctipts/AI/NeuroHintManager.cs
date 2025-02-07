using UnityEngine;
using Unity.Sentis;
using Game.Managers;
using System.Linq;
using System;
using System.Data;
using System.Runtime.CompilerServices;

namespace Game.AI
{
    public class NeuroHintManager : MonoBehaviour
    {
        #region CONSTANTS

        private const int SIZE_TENSOR = 81;

        #endregion

        #region CORE

        [Header("Core")]
        [SerializeField] private ModelAsset modelAsset; // ONNX модель

        private Model _runtimeModel;
        private Worker _worker;

        private Tensor<float> _inputTensor;
        private Tensor<float> _outputTensor;
        private TensorShape _shape;

        [Header("Managers")]
        [SerializeField] private GridManager gridManager;

        #endregion

        #region MONO

        private void Awake()
        {
            InitializeValues();
        }

        void OnDestroy()
        {
            _worker?.Dispose();
        }

        #endregion

        #region INITIALIZATION

        private void InitializeValues()
        {
            _runtimeModel = ModelLoader.Load(modelAsset);
            _worker = new Worker(_runtimeModel, BackendType.GPUCompute);

            _shape = new(SIZE_TENSOR);
        }

        #endregion

        #region CORE LOGIC

        public NeuroHint[] GenerateHints(int Count)
        {
            CreateInputTensor();
            NeuroHint[] neuroHints = CreateOutputTensor(Count);
            ClearMemory();

            return neuroHints;
        }

        private void CreateInputTensor()
        {
            int[,] puzzle = gridManager.Sudoku.RealGrid;
            float[] puzzleFlattened = FlattenGrid(puzzle);

            _inputTensor = new(_shape, puzzleFlattened);
        }

        private NeuroHint[] CreateOutputTensor(int Count)
        {
            _worker.Schedule(_inputTensor);

            NeuroHint[] neuroHints = new NeuroHint[Count];
            for (int i = 0; i < Count; i++)
            {
                NeuroHint neuroHint;
                do
                {
                    neuroHint = GetHintFromOutput();
                }
                while (neuroHints.Where(h => h != null).Any(hint => hint.Block == neuroHint.Block && hint.Number == neuroHint.Number));

                neuroHints[i] = neuroHint;
            }

            return neuroHints;
        }

        private void ClearMemory()
        {
            _inputTensor.Dispose();
            _outputTensor.Dispose();
        }

        #endregion

        #region CONVERT

        private float[] FlattenGrid(int[,] grid)
        {
            float[] flattened = new float[81];
            int index = 0;

            for (int blockRow = 0; blockRow < 3; blockRow++)
            {
                for (int blockCol = 0; blockCol < 3; blockCol++)
                {
                    for (int row = 0; row < 3; row++)
                    {
                        for (int col = 0; col < 3; col++)
                        {
                            int actualRow = blockRow * 3 + row;
                            int actualCol = blockCol * 3 + col;

                            flattened[index] = grid[actualRow, actualCol];
                            index++;
                        }
                    }
                }
            }

            return flattened;
        }

        #endregion

        #region GET

        private NeuroHint GetHintFromOutput()
        {
            _outputTensor = _worker.PeekOutput() as Tensor<float>;
            float[,] output2DArray = Get2DFloatArray(_outputTensor.DownloadToArray());  // (1,81,9) -> (81,9)
            int cell = 0;

            int[,] realGrid = gridManager.Sudoku.RealGrid;
            int block = 0;
            int number = 0;
            while (realGrid[block, number] != 0)
            {
                cell = UnityEngine.Random.Range(0, 81);

                int row = cell / 9;
                int col = cell % 9;

                block = GetBlockByRowAndCol(row, col);
                number = GetNumberByRowAndCol(row, col);
            }

            float[] probabilities = Enumerable.Range(0, 9).Select(j => output2DArray[cell, j]).ToArray();
            float maxProbability = probabilities.Max();

            int value = Array.IndexOf(probabilities, maxProbability) + 1; // от 1 до 9

            return new(value, block, number, maxProbability);
        }

        private float[,] Get2DFloatArray(float[] grid1D)
        {
            float[,] grid2D = new float[81, 9];
            for (int i = 0; i < 81; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    grid2D[i, j] = grid1D[i * 9 + j];
                }
            }

            return grid2D;
        }

        private int GetBlockByRowAndCol(int row, int col)
        {
            int startBlockY = row / 3 * 3;                  // 0, 3, 6
            int startBlockX = col / 3;                      // 0, 1, 2

            return startBlockY + startBlockX;          // от 0 до 8
        }

        private int GetNumberByRowAndCol(int row, int col)
        {
            int startNumberY = row % 3;                     // 0, 1, 2
            int startNumberX = col % 3;                     // 0, 1, 2

            return startNumberY * 3 + startNumberX;   // от 0 до 8
        }

        #endregion
    }
}