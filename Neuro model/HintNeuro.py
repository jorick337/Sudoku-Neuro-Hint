import torch
import torch.nn as nn
import torch.optim as optim
import numpy as np
import GridCreation as gc

# Архитектура нейросети для подсказки
class SudokuHintNet(nn.Module):
    def __init__(self):
        super(SudokuHintNet, self).__init__()
        self.conv1 = nn.Conv2d(1, 32, kernel_size=3, padding=1)  # 1 канал -> 32 канала
        self.conv2 = nn.Conv2d(32, 64, kernel_size=3, padding=1)  # 32 канала -> 64 канала
        self.fc1 = nn.Linear(64 * 9 * 9, 512)  # Полносвязный слой
        self.fc2 = nn.Linear(512, 81 * 9)  # Выходной слой (81 ячейка * 9 чисел)
        self.dropout = nn.Dropout(0.5)

    def forward(self, x):
        x = x.view(-1, 1, 9, 9)  # Преобразуем вход в 4D тензор
        x = torch.relu(self.conv1(x))
        x = torch.relu(self.conv2(x))
        x = x.view(-1, 64 * 9 * 9)  # Выравниваем для полносвязного слоя
        x = torch.relu(self.fc1(x))
        x = self.dropout(x)
        x = self.fc2(x)
        x = x.view(-1, 81, 9)  # Преобразуем в (batch_size, 81, 9)
        return x

# Создание модели
model = SudokuHintNet()

# Генерация данных
puzzles, solutions = gc.generate_valid_sudoku_data(num_samples=1000)

# Преобразование данных в тензоры PyTorch
puzzles_tensor = torch.tensor(puzzles, dtype=torch.float32)
solutions_tensor = torch.tensor(solutions - 1, dtype=torch.long)  # Целевые значения (от 0 до 8)

# Функция потерь и оптимизатор
criterion = nn.CrossEntropyLoss()
optimizer = optim.Adam(model.parameters(), lr=0.001)

# Обучение модели
batch_size = 32
num_epochs = 10

# for epoch in range(num_epochs):
#     for i in range(0, len(puzzles_tensor), batch_size):
#         batch_puzzles = puzzles_tensor[i:i+batch_size]
#         batch_solutions = solutions_tensor[i:i+batch_size]
        
#         optimizer.zero_grad()
#         output = model(batch_puzzles)
#         loss = criterion(output.view(-1, 9), batch_solutions.view(-1))
#         loss.backward()
#         optimizer.step()
    
#     print(f'Epoch {epoch+1}, Loss: {loss.item()}')

# Сохранение модели
torch.save(model.state_dict(), "sudoku_hint_net.pth")

# Функция для подсказки
def get_hint(model, puzzle):
    model.eval()
    with torch.no_grad():
        puzzle_tensor = torch.tensor(puzzle, dtype=torch.float32).unsqueeze(0)
        output = model(puzzle_tensor)
        output = output.view(81, 9)
        # Находим пустые ячейки
        empty_cells = [i for i, val in enumerate(puzzle) if val == 0]
        # Выбираем ячейку с наибольшей уверенностью
        best_cell = empty_cells[torch.argmax(output[empty_cells].max(dim=1)[0]).item()]
        best_number = torch.argmax(output[best_cell]).item() + 1
    return best_cell, best_number

# Тестирование на примере
puzzle, solution = gc.generate_valid_sudoku_data(num_samples=1)
print("Puzzle:")
print(puzzle[0].reshape(9, 9))
cell, number = get_hint(model, puzzle[0])
row, col = divmod(cell, 9)
print(f"Hint: Put {number} in cell ({row}, {col})")
print("Solution:")
print(solution[0].reshape(9, 9))