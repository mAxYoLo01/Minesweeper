#!/usr/bin/env python3

import random


class Minesweeper:
    def __init__(self, grid_size, mines_size):
        self.grid_size = grid_size
        self.mines_size = mines_size
        self.mines_grid = self.create_mines_grid(grid_size, mines_size)
        self.numbers_grid = self.create_numbers_grid(self.mines_grid)

    def create_mines_grid(self, grid_size, mines_size):
        grid = [[0 for _ in range(grid_size)] for _ in range(grid_size)]
        for _ in range(mines_size):
            while True:
                x = random.randint(0, grid_size - 1)
                y = random.randint(0, grid_size - 1)
                if not grid[x][y] == 1:
                    grid[x][y] = 1
                    break
        return grid

    def create_numbers_grid(self, mines_grid):
        numbers_grid = [[0 for _ in range(len(mines_grid))] for _ in range(len(mines_grid))]
        positions = [[1, 1], [1, 0], [1, -1], [0, -1], [-1, -1], [-1, 0], [-1, 1], [0, 1]]
        for i, row in enumerate(mines_grid):
            for j, cell in enumerate(row):
                cell_count = 0
                for pos in positions:
                    x = pos[0] + i
                    y = pos[1] + j
                    if x > -1 and x < len(mines_grid) and y > -1 and y < len(row):
                        if mines_grid[x][y] == 1:
                            cell_count += 1
                numbers_grid[i][j] = cell_count
        return numbers_grid

    def print_grid(self, grid):
        for row in grid:
            print(' '.join(str(cell) for cell in row), end='\n')
