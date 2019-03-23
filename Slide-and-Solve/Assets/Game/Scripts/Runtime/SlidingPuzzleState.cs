using UnityEngine;

namespace Wokarol.PuzzleProcessors
{
    public class SlidingPuzzleState
    {
        public enum StateType {None, Win}

        public readonly Vector2Int PlayerCoords;
        public readonly StateType Type;

        public SlidingPuzzleState(Vector2Int playerCoords, StateType type = StateType.None) {
            PlayerCoords = playerCoords;
            Type = type;
        }
    }
}