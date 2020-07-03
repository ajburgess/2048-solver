using System;
using System.Collections.Generic;
using System.Linq;

namespace _2048_Solver
{
    public class MonteCarloStrategy : IStrategy
    {
        private static Direction[] allDirections = (Direction[])Enum.GetValues(typeof(Direction));

        private IStrategy randomStrategy;
        private int size;

        public MonteCarloStrategy(int size)
        {
            this.randomStrategy = new RandomStrategy();
            this.size = size;
        }

        public bool TryPickNextMove(Game game, out Direction direction)
        {
            var outcomes = new List<(Direction direction, Game game)>();

            foreach (Direction d in allDirections)
            {
                for (int i = 0; i < size; i++)
                {
                    Game clone = game.Clone();
                    if (!clone.TryPlayMove(d))
                        break;
                    clone.PlayUntilConditionMetOrGameOver(randomStrategy, null, null);
                    outcomes.Add((d, clone));
                }
            }

            if (outcomes.Count == 0)
            {
                direction = Direction.Up;
                return false;
            }

            direction = outcomes
                .GroupBy(f => f.direction)
                .OrderByDescending(g => g.Average(f => f.game.Score))
                .First().Key;

            return true;
        }
    }
}