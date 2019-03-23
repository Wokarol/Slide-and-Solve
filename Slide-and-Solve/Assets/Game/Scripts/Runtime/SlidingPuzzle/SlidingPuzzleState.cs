using System.Collections.Generic;
using UnityEngine;

namespace Wokarol.PuzzleProcessors
{
    /// <summary>
    /// Stores info about state of a puzzle including any moving or changing part
    /// </summary>
    public struct SlidingPuzzleState
    {
        public enum StateType {Idle, Win}

        public readonly Vector2Int PlayerCoords;
        public readonly StateType Type;

        public SlidingPuzzleState(Vector2Int playerCoords, StateType type = StateType.Idle) {
            PlayerCoords = playerCoords;
            Type = type;
        }

        public static bool operator ==(SlidingPuzzleState a, SlidingPuzzleState b) => a.Equals(b);
        public static bool operator !=(SlidingPuzzleState a, SlidingPuzzleState b) => !a.Equals(b);

        public override bool Equals(object obj) {
            if (!(obj is SlidingPuzzleState)) {
                return false;
            }

            var state = (SlidingPuzzleState)obj;
            return PlayerCoords.Equals(state.PlayerCoords) &&
                   Type == state.Type;
        }

        public override int GetHashCode() {
            var hashCode = -1055164333;
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2Int>.Default.GetHashCode(PlayerCoords);
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            return hashCode;
        }

        public override string ToString() {
            return $"{PlayerCoords} [{Type}]";
        }
    }
}