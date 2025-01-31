import torch
import torch.nn as nn
import torch.optim as optim
from torch.utils.data import DataLoader, Dataset
import numpy as np
import random
import GridCreation
import Trainning

# Загрузка модели
class SudokuNet(nn.Module):
    def __init__(self):
        super(SudokuNet, self).__init__()
        self.fc1 = nn.Linear(81, 256)
        self.fc2 = nn.Linear(256, 81)

    def forward(self, x):
        x = torch.relu(self.fc1(x))
        x = self.fc2(x)
        return x

# Тестовый датасет
class SudokuDataset(Dataset):
    def __init__(self, puzzles, solutions):
        self.puzzles = puzzles  # Сетки судоку (входы)
        self.solutions = solutions  # Решения судоку (правильные значения)

    def __len__(self):
        return len(self.puzzles)

    def __getitem__(self, idx):
        puzzle = self.puzzles[idx]
        solution = self.solutions[idx]
        
        # Возвращаем:
        # 1. Вход (нерешённая сетка)
        # 2. Таргет (решённая сетка)
        return torch.tensor(puzzle, dtype=torch.float32), torch.tensor(solution, dtype=torch.float32)

# Загрузка обученной модели
def load_model():
    model = SudokuNet()
    model.load_state_dict(torch.load("sudoku_model.pth"))
    model.eval()  # Переводим модель в режим оценки
    return model

# Генерация тестовых данных
def generate_test_data(num_samples=100):
    puzzles, solutions = Trainning.generate_valid_sudoku_data()
    return SudokuDataset(puzzles, solutions)

# Функция для вычисления точности
def evaluate_model(model, dataloader):
    total_loss = 0.0
    criterion = nn.MSELoss()

    with torch.no_grad():
        for inputs, targets in dataloader:
            inputs = inputs.view(inputs.size(0), -1)  # [batch_size, 81]
            targets = targets.view(targets.size(0), -1)  # [batch_size, 81]

            outputs = model(inputs)  # Предсказания модели

            # Вычисляем потерю
            loss = criterion(outputs, targets)
            total_loss += loss.item()

    average_loss = total_loss / len(dataloader)
    return average_loss

# Основная функция
def main():
    # Загрузка модели
    model = load_model()

    # Генерация тестовых данных
    test_dataset = generate_test_data(num_samples=100)
    test_dataloader = DataLoader(test_dataset, batch_size=32, shuffle=False)

    # Оценка модели
    loss = evaluate_model(model, test_dataloader)
    print(f"Средняя ошибка на тестовых данных: {loss}")

# Запуск проверки модели
main()