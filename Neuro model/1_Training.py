import torch
import torch.nn as nn
import torch.optim as optim
import numpy as np
import GridCreation as gc

def generate_valid_sudoku_block(num_samples=1000):
    puzzles = []
    solutions = []

    for _ in range(num_samples):
        puzzle = np.zeros(9)
        solution = gc.generate_sudoku_block()
        
        puzzles.append(puzzle)
        solutions.append(solution)

    return np.array(puzzles), np.array(solutions)

# Класс нейросети со слоями, где:
# Вход - 9 значений
# Скрытый слой - 81 значений
# Выход - 9 значений
class SimpleSudokuNet(nn.Module):
    def __init__(self):
        super(SimpleSudokuNet, self).__init__()
        
        self.fc1 = nn.Linear(9, 81)     # Вход
        self.fc2 = nn.Linear(81, 81)    # Выход
        
        self.relu = nn.ReLU()               # Активация 1

    def forward(self, x):
        x = self.fc1(x)         # Выход первого слоя
        x = self.relu(x)        # Активация
        x = self.fc2(x)         # Выходной слой
        
        return x.view(-1,9,9)
    
# Функция потерь: бинарная кросс-энтропийная потеря
def get_loss(predictions, targets):
    criterion = nn.CrossEntropyLoss()

    true = predictions.argmax(dim=1).tolist()   # предсказанные значения
    print(true)
    
    targets = targets.long() - 1    # индексы

    return criterion(predictions, targets)

# Тренировка модели
def train_simple_model(puzzles, solutions, num_epochs=1):
    puzzles = torch.tensor(puzzles, dtype=torch.float32)
    solutions = torch.tensor(solutions, dtype=torch.long)

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

puzzles, solutions = generate_valid_sudoku_block(num_samples=1)
train_simple_model(puzzles, solutions, num_epochs=100)