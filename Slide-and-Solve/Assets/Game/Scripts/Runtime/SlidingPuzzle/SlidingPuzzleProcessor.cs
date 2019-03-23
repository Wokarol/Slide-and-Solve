using UnityEngine;

namespace Wokarol.PuzzleProcessors
{
    /// <summary>
    /// Processes puzzle
    /// </summary>
    public class SlidingPuzzleProcessor : IPuzzleProcessor<Vector2Int, SlidingPuzzleState>
    {
        SlidingPuzzleMap _map;

        public SlidingPuzzleProcessor(SlidingPuzzleMap map) {
            _map = map;
        }

        public SlidingPuzzleState Process(SlidingPuzzleState state, Vector2Int dir) {
            Vector2Int playerPos = state.PlayerCoords;
            while (CanGoInDirection()) {
                playerPos += dir;
            }

            if(playerPos == _map.WinCoords)
                return new SlidingPuzzleState(playerPos, SlidingPuzzleState.StateType.Win);
            return new SlidingPuzzleState(playerPos);

            bool CanGoInDirection() {
                Vector2Int nextPos = playerPos + dir;
                return 
                    nextPos.x >= 0 && nextPos.x < _map.Walls.GetLength(0) &&
                    nextPos.y >= 0 && nextPos.y < _map.Walls.GetLength(1) &&
                    !_map.Walls[nextPos.x, nextPos.y];
            }
        }

        //public struct Result
        //{
        //    public readonly SlidingPuzzleState State;

        //    public Result(SlidingPuzzleState state) {
        //        State = state;
        //    }
        //}
    } 
}
