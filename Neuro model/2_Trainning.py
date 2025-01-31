import torch
import torch.nn as nn
import torch.optim as optim
from torch.utils.data import DataLoader, Dataset
import numpy as np
import random
import GridCreation

# Генерирует такое-то кол-во примеров(равное num_samples) с:
# puzzles - пустые сетки судоку
# solutions - полные сетки судоку
def generate_valid_sudoku_data(num_samples=1000):
    puzzles = []
    solutions = []

    for _ in range(num_samples):
        puzzle = np.zeros(81)
        solution = GridCreation.generate_full_sudoku()
        
        puzzles.append(puzzle)
        solutions.append(solution)

    return np.array(puzzles), np.array(solutions)

# Класс нейросети со слоями, где:
# Вход - 81 значение
# Скрытый слой - 256 значений
# Выход - 81 значение
class SimpleSudokuNet(nn.Module):
    def __init__(self):
        super(SimpleSudokuNet, self).__init__()
        
        self.fc1 = nn.Linear(81, 256)           # Вход
        self.fc2 = nn.Linear(256, 512)         # Скрытый слой
        self.fc3 = nn.Linear(512, 81 * 9)    # Выход
        
        self.relu = nn.ReLU()               # Активация 1
        self.softmax = nn.Softmax(dim=1)    # Активация 2

    def forward(self, x):
        x = self.fc1(x)         # Первый слой
        x = self.relu(x)        # Активация
        x = self.fc2(x)         # Второй слой
        x = self.relu(x)        # Активация
        x = self.fc3(x)         # Генерация чисел
        
        return self.softmax(x)

# Функция потерь: бинарная кросс-энтропийная потеря
def get_loss(predictions, targets):
    criterion = nn.CrossEntropyLoss()
    
    predictions = predictions.view(-1,81,9)
    targets = targets.view(-1,81)

    return criterion(predictions, targets)

# Тренировка модели
def train_simple_model(puzzles, solutions, num_epochs=1000):
    puzzles = torch.tensor(puzzles, dtype=torch.float32)
    solutions = torch.tensor(solutions, dtype=torch.long) - 1   # Индексы от 0 до 8

    model = SimpleSudokuNet()
    optimizer = optim.Adam(model.parameters(), lr=0.001)

    for epoch in range(num_epochs):
        model.train()
        optimizer.zero_grad()

        predictions = model(puzzles)
        print(solutions)
        
        loss = get_loss(predictions, solutions)

        loss.backward()
        optimizer.step()

        print(f"Epoch [{epoch+1}/{num_epochs}], Loss: {loss.item():.4f}")

    torch.save(model.state_dict(), "simple_sudoku_model.pth")

puzzles, solutions = generate_valid_sudoku_data(num_samples=1)
train_simple_model(puzzles, solutions, num_epochs=150)