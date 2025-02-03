using UnityEngine;
using Unity.Sentis;
using Game.Managers;
using System.Text;
using Game.Classes;

namespace Game.AI
{
    public class SudokuHintModel : MonoBehaviour
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

            // Загрузка модели из ModelAsset
            _runtimeModel = ModelLoader.Load(modelAsset);

            // Создание Worker (выбор backend: GPU, CPU)
            _worker = new Worker(_runtimeModel, BackendType.GPUCompute);

            // Пример сетки судоку (одномерный массив)
            Sudoku s = gridManager.Sudoku;
            int[,] puzzle = s.RealGrid;

            // Преобразуем двумерный массив в одномерный
            float[] puzzleFlattened = FlattenGrid(puzzle);

            print("DSA");
            // Create a 3D tensor shape with size 3 × 1 × 3
            TensorShape shape = new TensorShape(81);

            // Create a new tensor from the array
            Tensor<float> inputTensor = new Tensor<float>(shape, puzzleFlattened);

            // Выполнение инференса
            _worker.Schedule(inputTensor);

            // Получение результата
            Tensor<float> outputTensor = _worker.PeekOutput() as Tensor<float>;
            string hint = GetHintFromOutput();

            // Лог результата
            Debug.Log(outputTensor);
            Debug.Log($"Подсказка для судоку: {hint}");

            StringBuilder sb = new StringBuilder();

            foreach (var item in puzzleFlattened)
            {
                sb.Append($"{item} ");
            }

            Debug.Log(sb.ToString());

            // Очистка ресурсов
            inputTensor.Dispose();
            outputTensor.Dispose();

            GenerateHint();
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

        private void GenerateHint()
        {
            CreateInputTensor();

            _worker.Schedule(_inputTensor);


            string hint = GetHintFromOutput();

            Debug.Log(hint);
            ClearMemory();
        }

        private void CreateInputTensor()
        {
            int[,] puzzle = gridManager.Sudoku.RealGrid;
            float[] puzzleFlattened = FlattenGrid(puzzle);

            _inputTensor = new(_shape, puzzleFlattened);
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

            for (int blockRow = 0; blockRow < 3; blockRow++)   // Проход по строкам блоков (0,1,2)
            {
                for (int blockCol = 0; blockCol < 3; blockCol++)   // Проход по столбцам блоков (0,1,2)
                {
                    for (int row = 0; row < 3; row++)   // Внутри блока - идём по строкам
                    {
                        for (int col = 0; col < 3; col++)   // Внутри блока - идём по столбцам
                        {
                            int actualRow = blockRow * 3 + row;  // Пересчёт в глобальные координаты
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

        private string GetHintFromOutput()
        {
            _outputTensor = _worker.PeekOutput() as Tensor<float>;
            float[] outputArray = _outputTensor.DownloadToArray(); // (81,9) -> (729,)

            // Форма тензора: (81, 9)
            int numCells = 81;
            int numNumbers = 9;

            // Находим ячейку и число с максимальной вероятностью
            int bestCell = 0;
            int bestNumber = 0;
            float maxProbability = 0;

            for (int cell = 0; cell < numCells; cell++)
            {
                for (int number = 0; number < numNumbers; number++)
                {
                    float probability = outputArray[cell * numNumbers + number];
                    if (probability > maxProbability)
                    {
                        maxProbability = probability;
                        bestCell = cell;
                        bestNumber = number + 1;    // Преобразуем в число от 1 до 9
                    }
                }
            }

            // индексы от 0 до 8
            int row = bestCell / 9; // ячейка
            int col = bestCell % 9; // столбец

            return $"{bestNumber} {row} {col}";
        }

        #endregion
    }
}