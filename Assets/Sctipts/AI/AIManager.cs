using UnityEngine;
using Unity.Sentis;
using Game.Managers;

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
        int[,] puzzle = GridManager.Instance.Sudoku.RealGrid;

        // Преобразуем двумерный массив в одномерный
        float[] puzzleFlattened = FlattenGrid(puzzle);

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
        Debug.Log($"Подсказка для судоку: {hint}");

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
                Debug.Log(flattened[i * 9 + j]);
            }
        }
        return flattened;
    }

    // Преобразование выходного тензора в подсказку
    private string GetHintFromOutput(Tensor<float> outputTensor)
    {
        float[] outputArray = outputTensor.DownloadToArray();

        int hintIndex = 0;
        float maxValue = outputArray[0];

        // Поиск индекса с максимальным значением
        for (int i = 1; i < outputArray.Length; i++)
        {
            if (outputArray[i] > maxValue)
            {
                maxValue = outputArray[i];
                hintIndex = i;
            }
        }

        // Индексы [0-8], подсказка [1-9]
        return (hintIndex + 1).ToString() + " " + (hintIndex % 9).ToString() + " " + (hintIndex / 9).ToString();
    }
}