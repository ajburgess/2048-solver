using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace _2048_Solver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();

            Direction[] allDirections = (Direction[])Enum.GetValues(typeof(Direction));

            Game game = Game.NewGame();

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
                        clone.PlayUntilEnd();
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

        static void Pause()
        {
            Thread.Sleep(20);
        }
    }
}
