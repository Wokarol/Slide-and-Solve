namespace Wokarol.PuzzleProcessors
{
    public interface IPuzzleProcessor<MoveT, StateT>
    {
        StateT Process(StateT state, MoveT move);
        bool StateIsWinning(StateT state);
    }
}