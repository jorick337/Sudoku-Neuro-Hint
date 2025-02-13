import torch
import onnx
import HintNeuro

# Загрузка обученной модели
model = HintNeuro.SudokuHintNet()
model.load_state_dict(torch.load("sudoku_hint_net.pth"))
model.eval()

# Пример входных данных (в формате тензора)
dummy_input = torch.zeros(81, dtype=torch.float32)  # Пустая сетка судоку

# Экспорт в ONNX
onnx_file = "sudoku_hint_model.onnx"
torch.onnx.export(
    model, 
    dummy_input, 
    onnx_file, 
    input_names=["input"], 
    output_names=["output"], 
    dynamic_axes={"input": {0: "batch_size"}, "output": {0: "batch_size"}}
)

print(f"Модель экспортирована в {onnx_file}")