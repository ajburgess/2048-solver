using System;
using System.Collections.Generic;

namespace _2048_Solver
{
    public class RandomStrategy : IStrategy
    {
        public bool TryPickNextMove(Game game, out Direction direction)
        {
            List<Direction> remainingDirections = new List<Direction>((Direction[])Enum.GetValues(typeof(Direction)));
            while (remainingDirections.Count > 0)
            {
                direction = RandomUtility.PickOneAndRemove(remainingDirections);
                if (game.TryPlayMove(direction))
                    return true;
            }

            direction = Direction.Up;
            return false;
        }
    }
}