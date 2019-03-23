using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.PuzzleProcessors;

namespace Wokarol.Pathfinder
{
    public class SlidingPuzzlePathinder
    {
        public Dictionary<SlidingPuzzleState, Node> BuildNodes(SlidingPuzzleProcessor processor, SlidingPuzzleState initialState) {
            var dict = new Dictionary<SlidingPuzzleState, Node>();
            var statesToCheck = new Queue<SlidingPuzzleState>();
            var posibbleMoves = new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) };

            statesToCheck.Enqueue(initialState);

            while (statesToCheck.Count > 0) {

                var state = statesToCheck.Dequeue();
                var neighbours = new List<Neighbour>();

                //Debug.Log($"================ {statesToCheck.Count}");
                //Debug.Log($"Checking state {state}");

                foreach (var move in posibbleMoves) {
                    var newState = processor.Process(state, move).State;
                    if (state != newState) {
                        Neighbour n = new Neighbour(move, newState);
                        neighbours.Add(n);
                        //Debug.Log($"\tAdded {n} as neighbour");
                        if (!statesToCheck.Contains(newState) && !dict.ContainsKey(newState)) {
                            //Debug.Log($"\t\tAdding it to queue");
                            statesToCheck.Enqueue(newState);
                        }
                    }
                }

                //Debug.Log($"Found {neighbours.Count} neighbours");


                if (!dict.ContainsKey(state)) {
                    //Debug.Log($"Dictionary does not contain my state");
                    dict.Add(state, new Node(neighbours.ToArray()));
                }
            }

            return dict;
        }

        public Vector2Int[] GetPath(Dictionary<SlidingPuzzleState, Node> nodes, SlidingPuzzleState startingState) {

            return new Vector2Int[0];
        }
    }

    /// <summary>
    /// Stores info about state of a puzzle and about posible moves
    /// </summary>
    public struct Node
    {
        public readonly Neighbour[] Neighbours;

        public Node(Neighbour[] neighbours) {
            Neighbours = neighbours;
        }
    }

    public struct Neighbour
    {
        public readonly Vector2Int Dir;
        public readonly SlidingPuzzleState State;

        public Neighbour(Vector2Int dir, SlidingPuzzleState state) {
            Dir = dir;
            State = state;
        }

        public override string ToString() {
            return $"[{Dir}]> {State}";
        }
    }
}
