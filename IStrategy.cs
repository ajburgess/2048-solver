namespace _2048_Solver
{
    public interface IStrategy
    {
        bool TryPickNextMove(Game game, out Direction direction);
    }
}