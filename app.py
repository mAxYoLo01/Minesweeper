#!/usr/bin/env python3

from minesweeper import Minesweeper


grid_size = 3
mines_size = 3
game = Minesweeper(grid_size, mines_size)
game.print_grid(game.mines_grid)
print('--------------------------------')
game.print_grid(game.numbers_grid)
# print(str(len([y for x in mines_grid for y in x if y == 1])) + " mines found!")
