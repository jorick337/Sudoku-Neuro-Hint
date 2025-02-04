using UnityEngine;
using Unity.Sentis;
using Game.Managers;
using System.Linq;
using System.Globalization;
using System;
using Game.Classes;

namespace Game.AI
{
    public class NeuroHintManager : MonoBehaviour
    {
        #region CONSTANTS

        private const int SIZE_TENSOR = 81;
        private const int AMOUNT_NEURO_HINTS = 4;

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

        public NeuroHint[] GenerateHints()
        {
            CreateInputTensor();
            NeuroHint[] neuroHints = CreateOutputTensor();
            ClearMemory();

            return neuroHints;
        }

        private void CreateInputTensor()
        {
            int[,] puzzle = gridManager.Sudoku.RealGrid;
            float[] puzzleFlattened = FlattenGrid(puzzle);

            _inputTensor = new(_shape, puzzleFlattened);
        }

        private NeuroHint[] CreateOutputTensor()
        {
            _worker.Schedule(_inputTensor);

            NeuroHint[] neuroHints = new NeuroHint[AMOUNT_NEURO_HINTS];
            for (int i = 0; i < AMOUNT_NEURO_HINTS; i++)
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
            float[] outputArray = _outputTensor.DownloadToArray(); // (1,81,9) -> (729)

            int[,] realGrid = gridManager.Sudoku.RealGrid;

            while (true)
            {
                int randomIndex = UnityEngine.Random.Range(0, 81);
                int block = randomIndex / 9;
                int number = randomIndex % 9;

                if (realGrid[block, number] != 0)
                {
                    int row = (number+1)/3;   // Строка в блоке(6/3 = 2, то 2*81 = 162 - начало третьей строки блока)
                    int col = block;
                    float[] probabilities = outputArray.Skip(block*81).Take(9).ToArray();
                    Debug.Log(probabilities.Sum());
                    float maxProbabilities = probabilities.Max();
                    int value = Array.IndexOf(probabilities, maxProbabilities) + 1;

                    return new(value, block, number, maxProbabilities);
                }
            }
        }

        #endregion
    }
}