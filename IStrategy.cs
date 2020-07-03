namespace _2048_Solver
{
    public interface IStrategy
    {
        bool TryMove(Game game, out Direction direction);
    }
}