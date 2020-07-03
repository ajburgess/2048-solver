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
                TryPlayMove(direction);

                // Allow caller to perform an action after each move
                if (afterMove != null)
                    afterMove();
            }
        }

        public bool TryPlayMove(Direction direction)
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
