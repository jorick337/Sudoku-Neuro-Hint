using UnityEngine;
using Unity.Sentis;
using System.Linq;
using System;
using System.Data;
using Help.Classes;
using Game.Classes;

namespace Game.Managers
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

        public NeuroHint[] GenerateHints(int count)
        {
            CreateInputTensor();
            NeuroHint[] neuroHints = CreateOutputTensor(count);
            ClearMemory();

            return neuroHints;
        }

        private void CreateInputTensor()
        {
            float[] puzzleFlattened = FlattenGrid(gridManager.Sudoku.RealGrid);

            _inputTensor = new(_shape, puzzleFlattened);
        }

        private NeuroHint[] CreateOutputTensor(int count)
        {
            _worker.Schedule(_inputTensor);
            _outputTensor = _worker.PeekOutput() as Tensor<float>;

            NeuroHint[] neuroHints = GetIndecesLargestValues(count);

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

        #region FIND

        private NeuroHint[] GetIndecesLargestValues(int count)
        {
            NeuroHint[] neuroHints = new NeuroHint[count];

            float[] output1DArray = _outputTensor.DownloadToArray();    // чтобы найти наибольшую вероятность
            float[,] output2DArray = Get2DFloatArray(output1DArray);    // чтобы удобнее искать блок и номер ячейки

            float[] maxProbabilities = output1DArray.OrderByDescending(probability => probability).ToArray();

            int i = 0;
            foreach (var probability in maxProbabilities)
            {
                int index = Array.IndexOf(output1DArray, probability) / 9;

                int row = index / 9;
                int col = index % 9;

                int block = GetBlockByRowAndCol(row, col);
                int number = GetNumberByRowAndCol(row, col);

                if (gridManager.Sudoku.RealGrid[block, number] == 0)
                {
                    float[] probabilities = Enumerable.Range(0, 9).Select(j => output2DArray[index, j]).ToArray();

                    if (i == 0)
                    {
                        (int, float) correctProbability = FindNeuroHint(true, probabilities, block, number, 0.1f, neuroHints);

                        if (correctProbability.Item1 == 0)
                        {
                            continue;
                        }

                        neuroHints[i] = new NeuroHint(correctProbability.Item1, correctProbability.Item2, block, number);
                        i++;
                    }
                    else
                    {
                        (int, float) incorrectProbability = FindNeuroHint(false, probabilities, block, number, 0.1f, neuroHints);

                        if (incorrectProbability.Item1 == 0)
                        {
                            continue;
                        }

                        neuroHints[i] = new NeuroHint(incorrectProbability.Item1, incorrectProbability.Item2, block, number);
                        i++;
                    }
                }

                if (i == count)
                    break;
            }

            return neuroHints;
        }

        private (int, float) FindNeuroHint(bool isCorrectly, float[] probabilities, int block, int number, float minProbability, NeuroHint[] neuroHints)
        {
            float maxProbability;
            int value;
            int index;

            do
            {
                maxProbability = probabilities.Max();

                if (maxProbability > minProbability)
                {
                    index = Array.IndexOf(probabilities, maxProbability);
                    value = index + 1;

                    if (gridManager.Sudoku.MainGrid[block, number] == value == isCorrectly && gridManager.Sudoku.RealGrid.IsSafe(block, number, value))
                    {
                        if (neuroHints[0] != null)
                        {
                            if (!neuroHints.Any(neuroHint => neuroHint?.Block == block && neuroHint?.Number == number))
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    value = 0;
                    break;
                }

                probabilities[index] = -1;
            }
            while (true);

            return (value, maxProbability);
        }

        #endregion

        #region GET

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