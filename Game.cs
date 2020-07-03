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

        // Creating a new game is a bit more complex than just creating an object,
        // so we use this factory method
        public static Game NewGame()
        {
            Game game = new Game();

            // Each new game starts with two numbers already on the grid
            game.grid.AddNewNumberInEmptyCell();
            game.grid.AddNewNumberInEmptyCell();

            return game;
        }

        private Game()
        {
            grid = new Grid();
        }

        // Use the given strategy to keep making moves, until either the game is over
        // (no more possible moves available) - or a particular condition has been met (e.g. 2048).
        // After each turn, call the supplied callback function
        // Returns true if game ended because of condition being met (else false)
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

        // Try to make the move, shifting cells and doubling numbers where appropriate
        // Update the score accordingly
        // Add another random number (2 or 4) in a random empty cell
        public bool TryMove(Direction direction)
        {
            // Shift the grid, track any doubling up that happens
            bool ok = grid.TryShift(direction, out int extraPoints);

            // Shift wasn't possible
            if (!ok)
                return false;

            // Player has taken another go
            moves++;

            // Keep track of points scored
            score += extraPoints;

            // New number goes into a random empty cell
            grid.AddNewNumberInEmptyCell();

            // Move was possible
            return true;
        }

        // At various points, we need to clone the game (and its current grid)
        // So we can evaluate moves, before returning to the original game again
        public Game Clone()
        {
            Game clone = new Game();

            clone.grid = grid.Clone(); // Need to make a deep copy of the grid array
            clone.score = score;
            clone.moves = moves;

            return clone;
        }
    }
}
