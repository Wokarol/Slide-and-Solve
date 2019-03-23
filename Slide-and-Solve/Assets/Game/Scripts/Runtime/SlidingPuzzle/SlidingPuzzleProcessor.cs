using System;
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
            return ProcessWithTransitions(state, dir).State;
        }

        public bool StateIsWinning(SlidingPuzzleState state) {
            return state.Type == SlidingPuzzleState.StateType.Win;
        }

        public Result ProcessWithTransitions(SlidingPuzzleState state, Vector2Int dir) {
            SlidingPuzzleState resultState;

            Vector2Int playerPos = state.PlayerCoords;
            while (CanGoInDirection()) {
                playerPos += dir;
            }

            if (playerPos == _map.WinCoords)
                resultState = new SlidingPuzzleState(playerPos, SlidingPuzzleState.StateType.Win);
            else
                resultState = new SlidingPuzzleState(playerPos);

            bool CanGoInDirection() {
                Vector2Int nextPos = playerPos + dir;
                return
                    nextPos.x >= 0 && nextPos.x < _map.Walls.GetLength(0) &&
                    nextPos.y >= 0 && nextPos.y < _map.Walls.GetLength(1) &&
                    !_map.Walls[nextPos.x, nextPos.y];
            }

            return new Result(resultState, new Transition[] { new Transition(state.PlayerCoords, playerPos) });
        }

        public struct Result
        {
            public readonly SlidingPuzzleState State;
            public readonly Transition[] transitions;

            public Result(SlidingPuzzleState state, Transition[] transitions) {
                State = state;
                this.transitions = transitions;
            }
        }


        public struct Transition
        {
            public readonly Vector2Int StartingCoords;
            public readonly Vector2Int EndCoords;

            public Transition(Vector2Int startingCoords, Vector2Int endCoords) {
                StartingCoords = startingCoords;
                EndCoords = endCoords;
            }
        }
    }
}
