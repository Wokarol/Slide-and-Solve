using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.PuzzleProcessors;

namespace Wokarol.Pathfinder
{
    public class PuzzlePathfinder<MoveT, StateT>
    {
        HashSet<Node> _winStates;
        List<Node> _graph;
        Dictionary<StateT, Node> _statesToGraphMap;

        public IReadOnlyList<Node> Graph => _graph;
        public IReadOnlyDictionary<StateT, Node> StatesToGraphMap => _statesToGraphMap;

        public PuzzlePathfinder<MoveT, StateT> RecalculateGrah(IPuzzleProcessor<MoveT, StateT> processor, StateT initialState, MoveT[] possibleMoves) {
            _statesToGraphMap = new Dictionary<StateT, Node>();
            _graph = new List<Node>();
            _winStates = new HashSet<Node>();
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
                if (processor.StateIsWinning(state))
                    _winStates.Add(node);
                _graph.Add(node);
                _statesToGraphMap.Add(state, node);
                closedSet.Add(state);

                // Iterating through posibble moves
                var neighbouringStates = new List<StateT>();
                var neighbourMoves = new List<MoveT>();
                foreach (var move in possibleMoves) {
                    var newState = processor.Process(state, move);
                    // State is not the same as original one and therefore this move changes state
                    if (!state.Equals(newState) && !neighbouringStates.Contains(newState)) {
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

        public Result GetClosestWinPath(StateT startingState) {
            if (!_statesToGraphMap.ContainsKey(startingState)) throw new ArgumentException($"startingState: {startingState} does not exist on graph\n{GetGraphText()}");

            return InteralFindPath(_statesToGraphMap[startingState], _winStates);
        }

        public Result GetPath(StateT startingState, StateT endingState) {
            if (!_statesToGraphMap.ContainsKey(startingState)) throw new ArgumentException($"startingState: {startingState} does not exist on graph\n{GetGraphText()}");
            if (!_statesToGraphMap.ContainsKey(endingState)) throw new ArgumentException($"endingState: {endingState} does not exist on graph\n{GetGraphText()}");

            return InteralFindPath(_statesToGraphMap[startingState], new HashSet<Node> {_statesToGraphMap[endingState]});
        }

        private Result InteralFindPath(Node startingNode, HashSet<Node> target) {
            List<Node> nodesToEvaluate = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();

            // Creating and filling dictionary of pathfinding data
            Dictionary<Node, PathFindingData> pathfindingData = new Dictionary<Node, PathFindingData>();
            foreach (var node in _graph) {
                pathfindingData.Add(node, new PathFindingData(null, int.MaxValue));
            }

            // Adding starting node
            pathfindingData[startingNode].ShortestPathLenght = 0;
            nodesToEvaluate.Add(startingNode);

            int safetyCheck = 100;
            while (nodesToEvaluate.Count > 0) {
                if (safetyCheck-- < 0) throw new Exception("Infinite Loop");

                var node = GetShortest(nodesToEvaluate, pathfindingData);
                var myData = pathfindingData[node];

                if (target.Contains(node)) {
                    //Debug.Log($"Found shortest path equal to {myData.ShortestPathLenght}");
                    var moves = TracePath(pathfindingData, node);
                    return new Result(moves, true);
                }

                nodesToEvaluate.Remove(node);
                closedSet.Add(node);

                //Debug.Log("=================");
                //Debug.Log($"Evaluating {node} with shortest distance {myData.ShortestPathLenght}");

                foreach (var neighbour in node.Neighbours) {
                    if (!nodesToEvaluate.Contains(neighbour.Node) && !closedSet.Contains(neighbour.Node)) {
                        //Debug.Log($"\tAdded {neighbour.Node} to nodes to evaluate");
                        nodesToEvaluate.Add(neighbour.Node);
                    }

                    var neighbourData = pathfindingData[neighbour.Node];
                    if (neighbourData.ShortestPathLenght > myData.ShortestPathLenght + 1) {
                        //Debug.Log($"\t\tMy path to this neighbour ({myData.ShortestPathLenght + 1}) is better than the old one {neighbourData.ShortestPathLenght}");
                        neighbourData.ShortestPathLenght = myData.ShortestPathLenght + 1;
                        neighbourData.Parent = node;
                    }
                }
            }

            return new Result(new MoveT[0], false);
        }

        private string GetGraphText() {
            List<string> lines = new List<string>();
            foreach (var item in _graph) {
                List<string> neighbours = new List<string>();
                foreach (var n in item.Neighbours) {
                    neighbours.Add($"{n.Node.State}");
                }
                lines.Add($"{item.State} -> {string.Join(", ", neighbours)}");
            }
            return string.Join("\n", lines);
        }

        private MoveT[] TracePath(Dictionary<Node, PathFindingData> pathfindingData, Node endNode) {
            Node currentNode = endNode;
            PathFindingData currentData = pathfindingData[currentNode];

            List<Node> path = new List<Node>();

            int safetyCheck = 100;
            while (currentData.Parent != null) {
                if (safetyCheck-- < 0) throw new Exception("Infinite Loop");

                path.Add(currentNode);

                currentNode = currentData.Parent;
                currentData = pathfindingData[currentNode];
            }
            path.Add(currentNode);
            path.Reverse();

            List<MoveT> moves = new List<MoveT>();

            for (int i = 0; i < path.Count - 1; i++) {
                moves.Add(path[i].GetNeighbour(path[i + 1]).Move);
            }

            List<string> nodeNames = new List<string>();
            foreach (var p in moves) {
                nodeNames.Add(p.ToString());
            }

            //Debug.Log($"{path.Count} nodes in path [{string.Join(" -> ", nodeNames)}]");
            return moves.ToArray();
        }

        Node GetShortest(List<Node> nodes, Dictionary<Node, PathFindingData> pathfindingData) {
            int shortest = int.MaxValue;
            Node shortestNode = null;
            for (int i = 0; i < nodes.Count; i++) {
                if (pathfindingData[nodes[i]].ShortestPathLenght < shortest) {
                    shortest = pathfindingData[nodes[i]].ShortestPathLenght;
                    shortestNode = nodes[i];
                }
            }
            return shortestNode;
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
            Dictionary<Node, Neighbour> _neighbourMap;

            public readonly StateT State;
            public Neighbour[] Neighbours { get; private set; }

            public Node(StateT state) {
                State = state;
            }

            public Node AssignNeighbours(Neighbour[] neighbours) {
                Neighbours = neighbours;

                _neighbourMap = new Dictionary<Node, Neighbour>();
                foreach (var neighbour in neighbours) {
                    _neighbourMap.Add(neighbour.Node, neighbour);
                }

                return this;
            }

            public Neighbour GetNeighbour(Node node) {
                if (_neighbourMap.ContainsKey(node))
                    return _neighbourMap[node];
                throw new ArgumentException("Node does not contain given neighbour");
            }

            public override string ToString() {
                return $"{State} [{Neighbours.Length} Neighbours]";
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

        private class PathFindingData
        {
            public PathFindingData(Node parent, int shortestPathLenght) {
                Parent = parent;
                ShortestPathLenght = shortestPathLenght;
            }

            public Node Parent { get; set; }
            public int ShortestPathLenght { get; set; }
        }
    }
}
