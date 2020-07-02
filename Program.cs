using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace _2048_Solver
{
    public static class RandomUtility
    {
        private static Random rnd = new Random();

        public static T PickOne<T>(IList<T> items)
        {
            if (items.Count == 0)
                throw new Exception("Empty items list");

            int index = rnd.Next(items.Count);

            return items[index];
        }

        public static T PickOneAndRemove<T>(IList<T> items)
        {
            T item = PickOne(items);
            items.Remove(item);
            return item;
        }

        public static double Between(double min, double max)
        {
            return rnd.Next() * (max - min) + min;
        }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class Game
    {
        private int[,] grid;
        private int score;
        private int moves;

        public int Score => score;

        public Game()
        {
            grid = new int[4, 4];
            score = 0;
            moves = 0;
        }

        public void Start()
        {
            if (EmptyPositions.Count() != 4 * 4)
                throw new Exception("Game has already started");

            AddNewNumberInEmptySpace();
            AddNewNumberInEmptySpace();
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

        public bool IsWon => PopulatedSquares.Any(s => s.Value >= 2048);

        public void Display()
        {
            Console.WriteLine($"Score: {score} Moves: {moves}");
            for (int row = 0; row < 4; row++)
            {
                Console.WriteLine("+----+----+----+----+");
                for (int col = 0; col < 4; col++)
                {
                    Console.Write("|");
                    Console.Write(grid[row, col].ToString().PadLeft(4).Replace("   0", "    "));
                    if (col == 3)
                    {
                        Console.WriteLine("|");
                    }
                }
                if (row == 3)
                {
                    Console.WriteLine("+----+----+----+----+");
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();

            Direction[] allDirections = (Direction[])Enum.GetValues(typeof(Direction));

            Game game = new Game();
            game.Start();

            while (true) {
                Console.SetCursorPosition(0, 0);
                game.Display();
                Pause();

                if (game.IsWon)
                    break;

                var futures = new List<(Direction direction, Game game)>();

                foreach (Direction direction in allDirections)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Game clone = game.Clone();
                        bool allowed = clone.TryMove(direction);
                        if (!allowed)
                            break;
                        PlayUntilEnd(clone);
                        futures.Add((direction, clone));
                    }
                }

                if (futures.Count == 0)
                    break;

                if (futures.Count > 0)
                {
                    Direction bestDirection = futures
                        .GroupBy(f => f.direction)
                        .OrderByDescending(g => g.Average(f => f.game.Score))
                        .First().Key;

                    game.TryMove(bestDirection);
                }
            }

            if (game.IsWon)
            {
                Console.WriteLine("You won!");
            }
            else
            {
                Console.WriteLine("You lost!");
            }
        }

        static void PlayUntilEnd(Game game)
        {
            while (true)
            {
                bool moved = false;
                List<Direction> remainingDirections = new List<Direction>((Direction[])Enum.GetValues(typeof(Direction)));
                while (remainingDirections.Count > 0)
                {
                    Direction direction = RandomUtility.PickOneAndRemove(remainingDirections);
                    bool allowed = game.TryMove(direction);
                    if (allowed)
                    {
                        moved = true;
                        break;
                    }
                }
                if (!moved) break;
            }
        }

        static void Pause()
        {
            Thread.Sleep(20);
        }
    }
}
