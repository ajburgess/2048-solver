using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace _2048_Solver
{
    public class MonteCarloStrategy : IStrategy
    {
        private int size;

        public MonteCarloStrategy(int size)
        {
            this.size = size;
        }

        private int PlayRandomUntilGameOver(Grid grid)
        {
            int score = 0;

            while (grid.TryRandomShift(out Direction _, out int points))
            {
                score += points;
                
                if (grid.Contains2048)
                    break;

                grid.AddNewNumberInEmptyCell();
            }

            return score;
        }

        public Direction PickBestMove(Grid grid)
        {
            Stack<Grid> grids = new Stack<Grid>();
            var outcomes = new List<(Direction direction, int score)>();

            foreach (Direction direction in Grid.AllDirections)
            {
                grids.Push(grid);
                grid = grid.Clone();
                if (grid.TryShift(direction, out int score))
                {
                    for (int i = 0; i < size; i++)
                    {
                        int score2 = score;
                        grids.Push(grid);
                        grid = grid.Clone();
                        grid.AddNewNumberInEmptyCell();
                        score2 += PlayRandomUntilGameOver(grid);
                        outcomes.Add((direction, score2));
                        grid = grids.Pop();
                    }
                }
                grid = grids.Pop();
            }

            if (outcomes.Count == 0)
            {
                return Direction.None;
            }

            var averages = outcomes
                .GroupBy(o => o.direction)
                .Select(g => new { direction = g.Key, averageScore = g.Average(o => o.score) })
                .OrderByDescending(g => g.averageScore);
 
            return averages.First().direction;
        }
    }
}