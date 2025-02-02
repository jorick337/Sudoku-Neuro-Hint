using UnityEngine;
using Unity.Sentis;
using Game.Managers;
using Game.Classes;
using System.Security.Cryptography;
using System.Text;

public class SudokuHintModel : MonoBehaviour
{
    public ModelAsset modelAsset; // ONNX модель, импортированная в Unity
    private Model runtimeModel;   // Модель, загруженная для инференса
    private Worker worker;        // Worker для выполнения инференса

    void Start()
    {
        // Загрузка модели из ModelAsset
        runtimeModel = ModelLoader.Load(modelAsset);

        // Создание Worker (выбор backend: GPU, CPU)
        worker = new Worker(runtimeModel, BackendType.GPUCompute);

        // Пример сетки судоку (одномерный массив)
        Sudoku s = new Sudoku(1);
        int[,] puzzle = s.RealGrid;

        // Преобразуем двумерный массив в одномерный
        float[] puzzleFlattened = FlattenGrid(puzzle);

        print("DSA");
        // Create a 3D tensor shape with size 3 × 1 × 3
        TensorShape shape = new TensorShape(81);

        // Create a new tensor from the array
        Tensor<float> inputTensor = new Tensor<float>(shape, puzzleFlattened);

        // Выполнение инференса
        worker.Schedule(inputTensor);

        // Получение результата
        Tensor<float> outputTensor = worker.PeekOutput() as Tensor<float>;
        string hint = GetHintFromOutput(outputTensor);

        // Лог результата
        Debug.Log(outputTensor);
        Debug.Log($"Подсказка для судоку: {hint}");

        float[] t = FlattenGrid(s.MainGrid);
        StringBuilder sb = new StringBuilder();
        int size = 9; // Для судоку 9x9

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                sb.Append(t[i * size + j] + " "); // Преобразуем одномерный индекс в двумерный
            }
            sb.AppendLine();
        }

        Debug.Log(sb.ToString());

        // Очистка ресурсов
        inputTensor.Dispose();
        outputTensor.Dispose();
    }

    void OnDestroy()
    {
        // Удаляем Worker при завершении работы
        worker?.Dispose();
    }

    // Преобразование двумерного массива в одномерный
    private float[] FlattenGrid(int[,] grid)
    {
        float[] flattened = new float[81];

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                flattened[i * 9 + j] = grid[i, j];
            }
        }
        return flattened;
    }

    // Преобразование выходного тензора в подсказку
    private string GetHintFromOutput(Tensor<float> outputTensor)
    {
        // Преобразуем тензор в массив
        float[] outputArray = outputTensor.DownloadToArray();

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
                    bestNumber = number + 1; // Преобразуем в число от 1 до 9
                }
            }
        }

        // Преобразуем индекс ячейки в строку и столбец
        int row = bestCell / 9;
        int col = bestCell % 9;

        // Возвращаем подсказку в формате: "число строка столбец"
        return $"{bestNumber} {row} {col}";
    }
}