namespace _2048_Solver
{
    public interface IStrategy
    {
        Direction PickBestMove(Grid grid);
    }
}