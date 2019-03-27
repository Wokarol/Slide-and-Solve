using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.Pathfinder;
using Wokarol.PuzzleProcessors;
using Wokarol.TilemapUtils;

namespace Wokarol
{
    public class GameBrain : MonoBehaviour
    {
        [SerializeField] MapDisplay _display = null;

        SlidingPuzzleProcessor _processor;
        PuzzlePathfinder<Vector2Int, SlidingPuzzleState> _pathfinder;
        SlidingPuzzleState currentState;

        Queue<Vector2Int> movementQueue;

        private void Start() {
            SlidingPuzzleMap map = new SlidingPuzzleMap(
                "11111111111111111111;" +
                "11111111110000011111;" +
                "11000000000111011111;" +
                "10000010000111010001;" +
                "10010010000000010001;" +

                "10111010110000000001;" +
                "10010000110101000001;" +
                "1100000000000101W101;" +
                "10000001100111011101;" +
                "10000001100000010001;" +

                "11110000000001100001;" +
                "11111000100001100001;" +
                "11111000100000000001;" +
                "11111000000000000001;" +
                "11110000000111111111;" +

                "11111100001111111111;" +
                "11111111111111111111"
                );
            currentState = new SlidingPuzzleState(new Vector2Int(7, 9));

            CreateMap(map);
            _display.ShowState(currentState);

            movementQueue = new Queue<Vector2Int>();

            _pathfinder = new PuzzlePathfinder<Vector2Int, SlidingPuzzleState>()
                .RecalculateGrah(_processor, currentState, new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right });
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                HandlePlayerInput(new Vector2Int(0, 1));
            }
            if (Input.GetKeyDown(KeyCode.DownArrow)) {
                HandlePlayerInput(new Vector2Int(0, -1));
            }
            if (Input.GetKeyDown(KeyCode.RightArrow)) {
                HandlePlayerInput(new Vector2Int(1, 0));
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                HandlePlayerInput(new Vector2Int(-1, 0));
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                var path = _pathfinder.GetClosestWinPath(currentState);
                movementQueue.Clear();
                for (int i = 0; i < path.Path.Length; i++) {
                    movementQueue.Enqueue(path.Path[i]);
                }
            }

            if (!_display.Moving && movementQueue.Count > 0) {
                HandleInput(movementQueue.Dequeue());
            }
        }

        private void HandlePlayerInput(Vector2Int dir) {
            movementQueue.Enqueue(dir);
        }

        private void HandleInput(Vector2Int dir) {
            if (!_display.Moving) {
                var result = _processor.ProcessWithTransitions(currentState, dir);

                currentState = result.State;
                if (_processor.StateIsWinning(currentState)) {
                    Debug.Log("You won");
                }
                _display.ShowTransition(result.Transitions, null); 
            }
        }

        private void CreateMap(SlidingPuzzleMap map) {
            if (_processor != null)
                _processor.Reconstruct(map);
            else
                _processor = new SlidingPuzzleProcessor(map);
            _display.SetMap(map);
        }
    }
}
