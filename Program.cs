﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace _2048_Solver
{
    class Program
    {
        static void Display(Game game)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Score: {game.Score} Moves: {game.Moves}");
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

        static void Main(string[] args)
        {
            Game game = Game.NewGame();

            IStrategy strategy = new MonteCarloStrategy(10);
            Func<bool> endCondition = () => game.Grid.AllValues.Any(v => v >= 2048);

            Console.Clear();
            Display(game);
            Thread.Sleep(10);

            game.PlayUntilConditionMetOrGameOver(strategy, endCondition, () =>
            {
                Display(game);
                Thread.Sleep(10);
            });

            bool won = game.Grid.AllValues.Any(v => v >= 2048);
            System.Console.WriteLine(won ? "You won!" : "You lost!");
        }
    }
}
