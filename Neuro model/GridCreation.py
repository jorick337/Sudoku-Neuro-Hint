import numpy as np
import random

# Генерация полностью заполненной сетки судоку в виде одномерного массива, где
# [0] - значения для строк блоков 1 2 3
def generate_full_sudoku():
    grid = np.zeros(81, dtype=int)  # Одномерный массив длиной 81

    def is_safe(num, row, col):
        if num in grid[row*9:(row+1)*9]:
            return False
        
        if num in grid[col::9]:
            return False
        
        start_row, start_col = 3 * (row // 3), 3 * (col // 3)
        for r in range(start_row, start_row + 3):
            for c in range(start_col, start_col + 3):
                if grid[r*9 + c] == num:
                    return False
        return True

    def fill_grid():
        for i in range(81):
            row, col = divmod(i, 9)
            if grid[i] == 0:
                nums = list(range(1, 10))
                random.shuffle(nums)
                for num in nums:
                    if is_safe(num, row, col):
                        grid[i] = num
                        if fill_grid():
                            return True
                        grid[i] = 0
                return False
        return True

    fill_grid()
    return grid

def generate_sudoku_block():
    numbers = list(range(1, 10))    # Список чисел от 1 до 9
    random.shuffle(numbers)         # Перемешиваем числа
    
    return numbers

# Удаляет определенное количество цифр из сетки
def remove_numbers(grid, num_holes):
    puzzle = grid.copy()
    holes = set()
    
    while len(holes) < num_holes:
        index = random.randint(0, 80)  # случайный индекс
        if index not in holes:
            puzzle[index] = 0
            holes.add(index)
            
    return puzzle

# Отображение сетку судоку в виде 9x9
def display_grid(grid):
    for i in range(9):
        for j in range(9):
            print(str(grid[i*9 + j]) if grid[i*9 + j] != 0 else '.', end=" ")
        print()

full_sudoku = generate_full_sudoku()
# puzzle = remove_numbers(full_sudoku, num_holes=random.randint(20, 40))
# display_grid(full_sudoku)
# print("Puzzle:")
# display_grid(puzzle)

def generate_valid_sudoku_data(num_samples=1000):
    puzzles = []
    solutions = []

    for _ in range(num_samples):
        solution = generate_full_sudoku()
        puzzle = remove_numbers(solution,36)
        
        puzzles.append(puzzle)
        solutions.append(solution)

    return np.array(puzzles), np.array(solutions)