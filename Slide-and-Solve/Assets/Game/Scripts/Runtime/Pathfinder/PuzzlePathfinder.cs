using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.PuzzleProcessors;

namespace Wokarol.Pathfinder
{
    public class PuzzlePathfinder<MoveT, StateT>
    {
        List<Node> _graph;
        Dictionary<StateT, Node> _statesToGraphMap;

        public IReadOnlyList<Node> Graph => _graph;
        public IReadOnlyDictionary<StateT, Node> StatesToGraphMap => _statesToGraphMap;

        public PuzzlePathfinder<MoveT, StateT> RecalculateGrah(IPuzzleProcessor<MoveT, StateT> processor, StateT initialState, MoveT[] possibleMoves) {
            _statesToGraphMap = new Dictionary<StateT, Node>();
            _graph = new List<Node>();
            GenerateNodes(processor, initialState, possibleMoves, out List<List<StateT>> neighbourMap, out List<List<MoveT>> neighbourMovesMap);
            FindNodeNeighbours(neighbourMap, neighbourMovesMap);
            return this;
        }

        /// <summary>
        /// Creates nodes from posibble states
        /// </summary>
        /// <param name="processor">Puzzle processor</param>
        /// <param name="initialState">Initial position to test all possible puzzle states</param>
        /// <param name="possibleMoves">List of moves that can be performed</param>
        /// <param name="neighbourMap">Map that corresponds to all states that are neighbour to given node, is same order as _graph</param>
        /// <param name="neighbourMovesMap">Map that correcponds to all moves needed to get to neighbour states, same sizes as neighbourMap</param>
        /// <returns>List of neighbouring states for each state</returns>
        private void GenerateNodes(IPuzzleProcessor<MoveT, StateT> processor, StateT initialState, MoveT[] possibleMoves, out List<List<StateT>> neighbourMap, out List<List<MoveT>> neighbourMovesMap) {
            var statesToCheck = new Queue<StateT>();
            var closedSet = new HashSet<StateT>();
            neighbourMap = new List<List<StateT>>();
            neighbourMovesMap = new List<List<MoveT>>();

            statesToCheck.Enqueue(initialState);

            // Looking through every possible state
            while (statesToCheck.Count > 0) {
                var state = statesToCheck.Dequeue();
                var node = new Node(state);
                _graph.Add(node);
                _statesToGraphMap.Add(state, node);
                closedSet.Add(state);

                // Iterating through posibble moves
                var neighbouringStates = new List<StateT>();
                var neighbourMoves = new List<MoveT>();
                foreach (var move in possibleMoves) {
                    var newState = processor.Process(state, move);
                    // State is not the same as original one and therefore is move changes anything
                    if (!state.Equals(newState)) {
                        // State was not tested and is not in the queue
                        if (!closedSet.Contains(newState) && !statesToCheck.Contains(newState)) {
                            statesToCheck.Enqueue(newState);
                        }
                        neighbouringStates.Add(newState);
                        neighbourMoves.Add(move);
                    }
                }
                neighbourMap.Add(neighbouringStates);
                neighbourMovesMap.Add(neighbourMoves);
            }
        }

        /// <summary>
        /// Converts neighbourMap and neighbourMovesMap to Neighbours of Nodes in _graph
        /// </summary>
        /// <param name="neighbourMap">Map that corresponds to all states that are neighbour to given node, is same order as _graph</param>
        /// <param name="neighbourMovesMap">Map that correcponds to all moves needed to get to neighbour states, same sizes as neighbourMap</param>
        private void FindNodeNeighbours(List<List<StateT>> neighbourMap, List<List<MoveT>> neighbourMovesMap) {
            if (neighbourMap.Count != _graph.Count) throw new ArgumentException("Not mathing neighbour map");
            for (int i = 0; i < _graph.Count; i++) {
                var node = _graph[i];
                var stateNeighbours = neighbourMap[i];
                var stateneighboursMoves = neighbourMovesMap[i];

                var neighbours = new List<Node.Neighbour>();
                for (int j = 0; j < stateNeighbours.Count; j++) {
                    neighbours.Add(new Node.Neighbour(
                        _statesToGraphMap[stateNeighbours[j]],
                        stateneighboursMoves[j]));
                }
                node.AssignNeighbours(neighbours.ToArray());
            }
        }

        public Result GetPath(StateT startingState, StateT endingState) {

            return new Result(new MoveT[0], false);
        }

        public class Result
        {
            public readonly MoveT[] Path;
            public readonly bool Succesfull;

            public Result(MoveT[] path, bool succesfull) {
                Path = path;
                Succesfull = succesfull;
            }
        }

        /// <summary>
        /// Represents a point on the navigation graph
        /// </summary>
        public class Node
        {
            public readonly StateT State;
            public Neighbour[] Neighbours { get; private set; }

            public Node(StateT state) {
                State = state;
            }

            public Node AssignNeighbours(Neighbour[] neighbours) {
                Neighbours = neighbours;
                return this;
            }

            /// <summary>
            /// Represents a path to neighbouring state
            /// </summary>
            public struct Neighbour
            {
                public readonly Node Node;
                public readonly MoveT Move;

                public Neighbour(Node node, MoveT move) {
                    Node = node;
                    Move = move;
                }
            }
        }
    }
}
