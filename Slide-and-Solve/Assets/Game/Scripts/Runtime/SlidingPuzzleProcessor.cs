using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wokarol.PuzzleProcessors
{
    public class SlidingPuzzleProcessor
    {
        SlidingPuzzleMap _map;

        public SlidingPuzzleProcessor(SlidingPuzzleMap map) {
            _map = map;
        }

        public Result Process(SlidingPuzzleState state, Vector2Int dir) {

            Vector2Int playerPos = state.PlayerCoords;

            Debug.Log($"size = ({_map.Walls.GetLength(0)}, {_map.Walls.GetLength(1)})");

            while (CanGoInDirection()) {
                Debug.Log("Incremented");
                playerPos += dir;
            }

            return new Result(new SlidingPuzzleState(playerPos));

            bool CanGoInDirection() {
                Vector2Int nextPos = playerPos + dir;
                Debug.Log(
                    $"Next position is {nextPos}\n" +
                    $"{ToText(_map.Walls)}\n" + 
                    $"{nextPos.x >= 0} && {nextPos.x < _map.Walls.GetLength(0)} && {nextPos.y >= 0} && {nextPos.y < _map.Walls.GetLength(1)} && {!_map.Walls[nextPos.x, nextPos.y]}");

                return 
                    nextPos.x >= 0 && nextPos.x < _map.Walls.GetLength(0) &&
                    nextPos.y >= 0 && nextPos.y < _map.Walls.GetLength(1) &&
                    !_map.Walls[nextPos.x, nextPos.y];

                string ToText(bool[,] map) {
                    string result = "";
                    for (int x = 0; x < map.GetLength(0); x++) {
                        result += $"x - {x} -> [";
                        for (int y = 0; y < map.GetLength(1); y++) {
                            result += map[x, y] ? "1" : "0";
                        }
                        result += "]\n";
                    }
                    return result;
                }
            }
        }

        public struct Result
        {
            public readonly SlidingPuzzleState State;

            public Result(SlidingPuzzleState state) {
                State = state;
            }
        }
    } 
}
