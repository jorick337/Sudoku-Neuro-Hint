import torch
import torch.nn as nn
import torch.optim as optim
import torch.nn.functional as F
import numpy as np
import GridCreation as gc

# Класс нейросети со слоями
class SimpleSudokuNet(nn.Module):
    def __init__(self):
        super(SimpleSudokuNet, self).__init__()
        
        self.fc1 = nn.Linear(81, 256)         # Входной слой (81 ячейка на входе)
        self.fc2 = nn.Linear(256, 512)        # Скрытый слой
        self.fc3 = nn.Linear(512, 512)        # Скрытый слой
        self.fc4 = nn.Linear(512, 81 * 9)

    def forward(self, x):
        x = F.relu(self.fc1(x))              
        x = F.relu(self.fc2(x))              
        x = F.relu(self.fc3(x))               
        x = self.fc4(x)
        
        x = x.view(-1, 81, 9)
        return F.softmax(x, dim=2)

# Функция потерь
def get_loss(predictions, targets):
    criterion = nn.CrossEntropyLoss()
    
    predictions = predictions.view(-1,9,81)
    targets = targets.view(-1,81)
    predicted_digits = nn.Softmax(dim=1)(predictions)
    predicted_digits = predicted_digits.argmax(dim=1) + 1
    print(predictions[0])
    print(predicted_digits[0])
    print(targets[0])

    return criterion(predictions, targets)

model = SimpleSudokuNet()
# Тренировка модели
def train_simple_model(puzzles, solutions, num_epochs=1000):
    puzzles = torch.tensor(puzzles, dtype=torch.float32)
    solutions = torch.tensor(solutions, dtype=torch.long) - 1
    
    optimizer = optim.Adam(model.parameters(), lr=0.001)

    for epoch in range(num_epochs):
        model.train()
        optimizer.zero_grad()

        predictions = model(puzzles)
        loss = get_loss(predictions, solutions)
        
        predicted_digits = nn.Softmax(dim=1)(predictions)
        predicted_digits = predicted_digits.argmax(dim=1) + 1
        print(predicted_digits)
        
        loss.backward()
        optimizer.step()

        print(f"Epoch [{epoch+1}/{num_epochs}], Loss: {loss.item():.4f}")

    torch.save(model.state_dict(), "simple_sudoku_model.pth")

puzzles, solutions = gc.generate_valid_sudoku_data(num_samples=2)
train_simple_model(puzzles, solutions, num_epochs=150)

def solve_sudoku(model, sudoku_grid):
    model.eval()  # Переводим модель в режим оценки
    with torch.no_grad():
        output = model(sudoku_grid)
    
    predicted_digits = nn.Softmax(dim=2)(output)
    predicted_digits = predicted_digits.argmax(dim=2) + 1
    
    return predicted_digits

# puzzles = torch.tensor(puzzles, dtype=torch.float32)
# print(puzzles)
# s = solve_sudoku(model, puzzles)
# print(s)