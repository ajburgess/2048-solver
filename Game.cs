using System;
using System.Linq;
using System.Collections.Generic;

namespace _2048_Solver
{
    public class Game
    {
        private int[,] grid;
        private int score;
        private int moves;

        public int? this[int row, int col]
        {
            get
            {
                int value = grid[row, col];
                return value != 0 ? value : (int?)null;
            }
        }

        public int Score => score;
        public int Moves => moves;

        public static Game NewGame()
        {
            Game game = new Game();

            game.AddNewNumberInEmptySpace();
            game.AddNewNumberInEmptySpace();

            return game;
        }

        private Game()
        {
            grid = new int[4, 4];
        }

        public bool Contains(int value) => PopulatedSquares.Any(s => s.Value == value);

        public IEnumerable<int> AllValues => PopulatedSquares.Select(s => s.Value);

        public bool PlayUntilConditionMetOrGameOver(IStrategy strategy, Func<bool> condition, Action afterMove)
        {
            while (true)
            {
                // Stop if condition is met
                if (condition != null && condition())
                    return true;

                // Pick next move, using the supplied strategy
                Game clone = this.Clone();
                bool canMove = strategy.TryPickNextMove(clone, out Direction direction);

                // Stop if no move is possible
                if (!canMove)
                    return false;

                // Move in the chosen direction
                Move(direction);

                // Allow caller to perform an action after each move
                if (afterMove != null)
                    afterMove();
            }
        }

        public void Move(Direction direction)
        {
            bool ok = TryMove(direction);

            if (!ok)
                throw new Exception("Invalid move");
        }

        private IEnumerable<(int Row, int Col)> EmptyPositions =>
            AllSquares.Where(s => s.Value == 0).Select(s => (s.Row, s.Col));

        private IEnumerable<(int Row, int Col, int Value)> PopulatedSquares =>
            AllSquares.Where(s => s.Value != 0);

        private IEnumerable<(int Row, int Col, int Value)> AllSquares
        {
            get
            {
                for (int row = 0; row <= 3; row++)
                {
                    for (int col = 0; col <= 3; col++)
                    {
                        yield return (row, col, grid[row, col]);
                    }
                }
            }
        }

        private void AddNewNumberInEmptySpace()
        {
            var position = RandomUtility.PickOne(EmptyPositions.ToList());
            int number = RandomUtility.Between(0.0, 1.0) < 0.9 ? 2 : 4;
            grid[position.Row, position.Col] = number;
        }

        public Game Clone()
        {
            Game clone = new Game();

            foreach (var s in AllSquares)
            {
                clone.grid[s.Row, s.Col] = s.Value;
            }

            clone.score = score;
            clone.moves = moves;

            return clone;
        }

        private bool RowIsEmpty(int row)
        {
            return Enumerable.Range(0, 4).All(col => grid[row, col] == 0);
        }

        private bool ColumnIsEmpty(int col)
        {
            return Enumerable.Range(0, 4).All(row => grid[row, col] == 0);
        }

        public bool TryMove(Direction direction)
        {
            bool allowed = false;

            switch (direction)
            {
                case Direction.Left:
                    allowed = TryShiftLeft();
                    break;
                case Direction.Right:
                    allowed = TryShiftRight();
                    break;
                case Direction.Up:
                    allowed = TryShiftUp();
                    break;
                case Direction.Down:
                    allowed = TryShiftDown();
                    break;
            }

            if (allowed)
            {
                foreach (var s in PopulatedSquares)
                {
                    if (s.Value < 0)
                    {
                        int value = Math.Abs(s.Value);
                        grid[s.Row, s.Col] = value;
                        score += value;
                    }
                }

                AddNewNumberInEmptySpace();
                moves++;
            }

            return allowed;
        }

        private bool TryShiftLeft()
        {
            bool allowed = false;

            for (int row = 0; row <= 3; row++)
            {
                bool keepShifting = !RowIsEmpty(row);
                while (keepShifting)
                {
                    keepShifting = false;
                    for (int col = 1; col <= 3; col++)
                    {
                        if (grid[row, col] != 0)
                        {
                            if (grid[row, col] == grid[row, col - 1] && grid[row, col] > 0)
                            {
                                int newValue = grid[row, col] * 2;
                                grid[row, col - 1] = 0 - newValue;
                                grid[row, col] = 0;
                                keepShifting = true;
                                allowed = true;
                            }
                            else if (grid[row, col - 1] == 0)
                            {
                                grid[row, col - 1] = grid[row, col];
                                grid[row, col] = 0;
                                keepShifting = true;
                                allowed = true;
                            }
                        }
                    }
                }
            }

            return allowed;
        }

        private bool TryShiftRight()
        {
            bool allowed = false;

            for (int row = 0; row <= 3; row++)
            {
                bool keepShifting = !RowIsEmpty(row);
                while (keepShifting)
                {
                    keepShifting = false;
                    for (int col = 2; col >= 0; col--)
                    {
                        if (grid[row, col] != 0)
                        {
                            if (grid[row, col] == grid[row, col + 1] && grid[row, col] > 0)
                            {
                                int newValue = grid[row, col] * 2;
                                grid[row, col + 1] = 0 - newValue;
                                grid[row, col] = 0;
                                keepShifting = true;
                                allowed = true;
                            }
                            else if (grid[row, col + 1] == 0)
                            {
                                grid[row, col + 1] = grid[row, col];
                                grid[row, col] = 0;
                                keepShifting = true;
                                allowed = true;
                            }
                        }
                    }
                }
            }

            return allowed;
        }

        private bool TryShiftUp()
        {
            bool allowed = false;

            for (int col = 0; col <= 3; col++)
            {
                bool keepShifting = !ColumnIsEmpty(col);
                while (keepShifting)
                {
                    keepShifting = false;
                    for (int row = 1; row <= 3; row++)
                    {
                        if (grid[row, col] != 0)
                        {
                            if (grid[row, col] == grid[row - 1, col] && grid[row, col] > 0)
                            {
                                int newValue = grid[row, col] * 2;
                                grid[row - 1, col] = 0 - newValue;
                                grid[row, col] = 0;
                                keepShifting = true;
                                allowed = true;
                            }
                            else if (grid[row - 1, col] == 0)
                            {
                                grid[row - 1, col] = grid[row, col];
                                grid[row, col] = 0;
                                keepShifting = true;
                                allowed = true;
                            }
                        }
                    }
                }
            }

            return allowed;
        }

        private bool TryShiftDown()
        {
            bool allowed = false;

            for (int col = 0; col <= 3; col++)
            {
                bool keepShifting = !ColumnIsEmpty(col);
                while (keepShifting)
                {
                    keepShifting = false;
                    for (int row = 2; row >= 0; row--)
                    {
                        if (grid[row, col] != 0)
                        {
                            if (grid[row, col] == grid[row + 1, col] && grid[row, col] > 0)
                            {
                                int newValue = grid[row, col] * 2;
                                grid[row + 1, col] = 0 - newValue;
                                grid[row, col] = 0;
                                keepShifting = true;
                                allowed = true;
                            }
                            else if (grid[row + 1, col] == 0)
                            {
                                grid[row + 1, col] = grid[row, col];
                                grid[row, col] = 0;
                                keepShifting = true;
                                allowed = true;
                            }
                        }
                    }
                }
            }

            return allowed;
        }
    }
}
