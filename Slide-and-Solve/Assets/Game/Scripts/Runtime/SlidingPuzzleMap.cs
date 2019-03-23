using System.Collections.Generic;
using UnityEngine;

namespace Wokarol.PuzzleProcessors
{
    public class SlidingPuzzleMap
    {
        public readonly bool[,] Walls;
        public readonly Vector2Int WinCoords;

        public SlidingPuzzleMap(bool[,] walls, Vector2Int winCoords) {
            Walls = walls;
            WinCoords = winCoords;
        }
    }
}