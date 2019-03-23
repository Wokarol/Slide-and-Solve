using UnityEngine;

namespace Wokarol.PuzzleProcessors
{
    /// <summary>
    /// Stores all static info about puzzle map
    /// </summary>
    public class SlidingPuzzleMap
    {
        public readonly bool[,] Walls;
        public readonly Vector2Int WinCoords;

        public SlidingPuzzleMap(bool[,] walls, Vector2Int winCoords) {
            Walls = walls;
            WinCoords = winCoords;
        }

        public SlidingPuzzleMap(string source) {
            source = source.Trim();
            var rows = source.Split(';');
            int maxColsCount = 0;
            for (int i = 0; i < rows.Length; i++) {
                rows[i] = rows[i].Trim();
                if (rows[i].Length > maxColsCount) maxColsCount = rows[i].Length;
            }

            Walls = new bool[maxColsCount, rows.Length];
            for (int x = 0; x < maxColsCount; x++) {
                for (int y = 0; y < rows.Length; y++) {
                    Walls[x, y] = true;
                }
            }

            bool winExisted = false;

            for (int y = 0; y < rows.Length; y++) {
                for (int x = 0; x < rows[y].Length; x++) {
                    var col = rows[y].ToCharArray();
                    Walls[x, y] = col[x] == '1';
                    if (col[x] == 'W') {
                        if (winExisted) throw new System.ArgumentException($"There's more than one win on {WinCoords} and ({x}, {y})");
                        winExisted = true;
                        WinCoords = new Vector2Int(x, y);
                    }
                }
            }
            if(!winExisted) throw new System.ArgumentException($"There's no win");
        }
    }
}