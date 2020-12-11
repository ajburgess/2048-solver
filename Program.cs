using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace _2048_Solver
{
    class Program
    {
        static void Display(Grid grid, int score, int moves)
        {
            int maxValue = grid.AllValues.Max();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Score: {score} Moves: {moves} Max: {maxValue}          ");
            Console.WriteLine(grid);
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

        static void WriteHistoryHeader(StreamWriter sw)
        {
            sw.WriteLine("A1,A2,A3,A4,B1,B2,B3,B4,C1,C2,C3,C4,D1,D2,D3,D4,Move");
        }

        static void WriteHistory(StreamWriter sw, List<(Grid, Direction)> history)
        {
            foreach ((Grid grid, Direction move) in history)
            {
                bool isFirst = true;
                foreach (int value in grid.AllValues) {
                    if (!isFirst) {
                        sw.Write(",");
                    } else {
                        isFirst = false;
                    }
                    sw.Write(value);
                }
                sw.Write(",");
                sw.Write(move);
                sw.WriteLine();
            }
        }

        static void Main(string[] args)
        {
            int numGames = 2;
            bool display = true;
            IStrategy strategy = new MonteCarloStrategy(25);
            using (StreamWriter sw = File.CreateText("moves2.csv"))
            {
                WriteHistoryHeader(sw);
                for (int i = 0; i < numGames; i++)
                {
                    List<(Grid, Direction)> history = PlayGame(strategy, display, out bool won);
                    Console.WriteLine($"Game {i+1}: {(won ? "Won" : "Lost")}    ");
                    if (won)
                    {
                        WriteHistory(sw, history);
                        sw.Flush();
                    }
                }
            }
            Console.ReadLine();
        }

        static List<(Grid, Direction)> PlayGame(IStrategy strategy, bool display, out bool won)
        {
            Grid grid = new Grid();
            grid.AddNewNumberInEmptyCell();
            grid.AddNewNumberInEmptyCell();
            int score = 0;
            int moves = 0;
            won = false;

            List<(Grid, Direction)> history = new List<(Grid, Direction)>();

            if (display)
                Display(grid, score, moves);

            Direction move;
            while (!won && (move = strategy.PickBestMove(grid)) != Direction.None)
            {
                moves++;
                history.Add((grid.Clone(), move));
                grid.TryShift(move, out int points);
                score += points;

                if (grid.Contains2048)
                {
                    history.Add((grid.Clone(), Direction.None));
                    won = true;
                }
                else
                {
                    grid.AddNewNumberInEmptyCell();
                }

                if (display)
                    Display(grid, score, moves);
            }

            return history;
        }
    }
}

