using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace _2048_Solver
{
    class Program
    {
        static void Display(Game game)
        {
            int maxValue = game.Grid.AllValues.Max();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Score: {game.Score} Moves: {game.Moves} Max: {maxValue}");
            for (int row = 0; row <= 3; row++)
            {
                Console.WriteLine("+----+----+----+----+");
                for (int col = 0; col <= 3; col++)
                {
                    Console.Write("|");
                    Console.Write((game.Grid[row, col]?.ToString() ?? "").PadLeft(4));
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

        private static string GetDirectionSymbol(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left: return "\u2190";
                case Direction.Right: return "\u2192";
                case Direction.Up: return "\u2191";
                case Direction.Down: return "\u2193";
                default: return "?";
            }
        }

        private static void DisplayNextMove(Direction direction)
        {
            Console.WriteLine();
            string symbol = GetDirectionSymbol(direction);
            System.Console.Write($"Next move: {symbol}");
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void WaitForKeyPress()
        {
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            Game game = Game.NewGame();

            IStrategy strategy = new MonteCarloStrategy(10);
            Func<bool> endCondition = () => game.Grid.AllValues.Any(v => v >= 2048);

            Console.Clear();

            Display(game);

            game.PlayUntilConditionMetOrGameOver(strategy, endCondition, d =>
            {
                DisplayNextMove(d);
                WaitForKeyPress();
                Display(game);
                //Thread.Sleep(10);
            });

            bool won = game.Grid.AllValues.Any(v => v >= 2048);
            System.Console.WriteLine(won ? "You won!" : "You lost!");
        }
    }
}
