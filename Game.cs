using System;
using System.Linq;
using System.Collections.Generic;

namespace _2048_Solver
{
    public class Game
    {
        private Grid grid;
        private int score;
        private int moves;

        public Grid  Grid => grid;
        public int Score => score;
        public int Moves => moves;

        public static Game NewGame()
        {
            Game game = new Game();

            game.grid.AddNewNumberInEmptyCell();
            game.grid.AddNewNumberInEmptyCell();

            return game;
        }

        private Game()
        {
            grid = new Grid();
        }

        public bool PlayUntilConditionMetOrGameOver(IStrategy strategy, Func<bool> condition, Action<Direction> afterMove)
        {
            while (true)
            {
                // Stop if condition is met
                if (condition != null && condition())
                    return true;

                // Pick and play next move, using the supplied strategy
                bool canMove = strategy.TryMove(this, out Direction direction);

                // Stop if no move was possible
                if (!canMove)
                    return false;

                // Tell caller the direction of the move just played
                if (afterMove != null)
                    afterMove(direction);
            }
        }

        public bool TryMove(Direction direction)
        {
            bool ok = grid.TryShift(direction, out int extraPoints);

            if (!ok)
                return false;

            moves++;
            score += extraPoints;

            grid.AddNewNumberInEmptyCell();

            return true;
        }

        public Game Clone()
        {
            Game clone = new Game();

            clone.grid = grid.Clone();
            clone.score = score;
            clone.moves = moves;

            return clone;
        }
    }
}
