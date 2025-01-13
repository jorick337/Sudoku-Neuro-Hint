import torch
import torch.nn as nn
import torch.optim as optim
from torch.utils.data import DataLoader, Dataset
import numpy as np
import random


# Создание датасета
class SudokuDataset(Dataset):
    def __init__(self, puzzles, solutions):
        self.puzzles = puzzles  # Сетки судоку (входы)
        self.solutions = solutions  # Решения судоку (правильные значения)

    def __len__(self):
        return len(self.puzzles)

    def __getitem__(self, idx):
        puzzle = self.puzzles[idx]
        solution = self.solutions[idx]

        # Находим первую пустую ячейку
        for i in range(81):
            if puzzle[i] == 0:
                target = solution[i]  # Правильное значение для этой ячейки
                return torch.tensor(puzzle, dtype=torch.float32), target - 1
        return torch.tensor(puzzle, dtype=torch.float32), 0


# Архитектура модели
class SudokuHintNet(nn.Module):
    def __init__(self):
        super(SudokuHintNet, self).__init__()
        self.fc1 = nn.Linear(81, 128)
        self.fc2 = nn.Linear(128, 64)
        self.fc3 = nn.Linear(64, 9)

    def forward(self, x):
        x = torch.relu(self.fc1(x))
        x = torch.relu(self.fc2(x))
        x = self.fc3(x)
        return x


def generate_full_sudoku():
    """Создаёт полностью заполненную решённую сетку судоку."""
    grid = np.zeros((9, 9), dtype=int)

    def is_safe(num, row, col):
        # Проверяем строку
        if num in grid[row]:
            return False
        # Проверяем столбец
        if num in grid[:, col]:
            return False
        # Проверяем подблок 3x3
        start_row, start_col = 3 * (row // 3), 3 * (col // 3)
        if num in grid[start_row:start_row + 3, start_col:start_col + 3]:
            return False
        return True

    def fill_grid():
        for i in range(81):
            row, col = divmod(i, 9)
            if grid[row, col] == 0:
                nums = list(range(1, 10))
                random.shuffle(nums)
                for num in nums:
                    if is_safe(num, row, col):
                        grid[row, col] = num
                        if fill_grid():
                            return True
                        grid[row, col] = 0
                return False
        return True

    fill_grid()
    return grid

def remove_numbers(grid, num_holes):
    """Удаляет числа из полной сетки, чтобы создать головоломку."""
    puzzle = grid.copy()
    holes = set()
    while len(holes) < num_holes:
        row, col = random.randint(0, 8), random.randint(0, 8)
        if (row, col) not in holes:
            puzzle[row, col] = 0
            holes.add((row, col))
    return puzzle

def generate_valid_sudoku_data(num_samples=10):
    puzzles = []
    solutions = []

    for _ in range(num_samples):
        full_sudoku = generate_full_sudoku()
        puzzle = remove_numbers(full_sudoku, num_holes=random.randint(20, 40))  # 20–40 пустых ячеек
        puzzles.append(puzzle.flatten())
        solutions.append(full_sudoku.flatten())

    return np.array(puzzles), np.array(solutions)


# Обучение модели
def train_model():
    # Генерация данных
    puzzles, solutions = generate_valid_sudoku_data()

    dataset = SudokuDataset(puzzles, solutions)
    dataloader = DataLoader(dataset, batch_size=32, shuffle=True)

    # Создание модели
    model = SudokuHintNet()
    criterion = nn.CrossEntropyLoss()
    optimizer = optim.Adam(model.parameters(), lr=0.001)

    # Тренировка
    num_epochs = 1000
    for epoch in range(num_epochs):
        for batch in dataloader:
            inputs, targets = batch
            outputs = model(inputs)

            loss = criterion(outputs, targets.long())
            optimizer.zero_grad()
            loss.backward()
            optimizer.step()

        print(f"Epoch {epoch+1}/{num_epochs}, Loss: {loss.item()}")

    # Сохранение модели
    torch.save(model.state_dict(), "sudoku_hint_model.pth")
    print("Модель сохранена в sudoku_hint_model.pth")


# Пример использования
def use_model():
    # Загрузка обученной модели
    model = SudokuHintNet()
    model.load_state_dict(torch.load("sudoku_hint_model.pth"))
    model.eval()

    # Пример сетки судоку
    puzzle = np.array([
        5, 3, 0, 0, 7, 0, 0, 0, 0,
        6, 0, 0, 1, 9, 5, 0, 0, 0,
        0, 9, 8, 0, 0, 0, 0, 6, 0,
        8, 0, 0, 0, 6, 0, 0, 0, 3,
        4, 0, 0, 8, 0, 3, 0, 0, 1,
        7, 0, 0, 0, 2, 0, 0, 0, 6,
        0, 6, 0, 0, 0, 0, 2, 8, 0,
        0, 0, 0, 4, 1, 9, 0, 0, 5,
        0, 0, 0, 0, 8, 0, 0, 7, 9
    ])

    # Предсказание
    puzzle_tensor = torch.tensor(puzzle, dtype=torch.float32)
    with torch.no_grad():
        prediction = model(puzzle_tensor)
        hint = torch.argmax(prediction).item() + 1  # Значение для пустой ячейки
        print(f"Подсказка: {hint}")


if __name__ == "__main__":
    train_model()
    use_model()