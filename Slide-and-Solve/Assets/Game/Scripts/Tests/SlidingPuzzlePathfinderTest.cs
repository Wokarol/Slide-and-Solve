using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.PuzzleProcessors;
using System.Linq;

namespace Wokarol.Pathfinder
{
    public class SlidingPuzzlePathfinderTest
    {
        readonly Vector2Int _up = new Vector2Int(0, -1);
        readonly Vector2Int _down = new Vector2Int(0, 1);
        readonly Vector2Int _left = new Vector2Int(-1, 0);
        readonly Vector2Int _right = new Vector2Int(1, 0);

        private SlidingPuzzleProcessor _processor;
        private SlidingPuzzlePathinder _pathfinder;
        private Dictionary<SlidingPuzzleState, Node> _nodes;

        [SetUp]
        public void Setup() {
            var map = new SlidingPuzzleMap(
                "111111;" +
                "100011;" +
                "110001;" +
                "100001;" +
                "100011;" +
                "11W111;" +
                "111111");
            _processor = new SlidingPuzzleProcessor(map);
            _pathfinder = new SlidingPuzzlePathinder();
            _nodes = _pathfinder.BuildNodes(_processor, new SlidingPuzzleState(new Vector2Int(1, 4)));
        }

        // (-?\d) (-?\d)
        // new SlidingPuzzleState(new Vector2Int($1, $2))

        // (_\w+) (-?\d -?\d)
        // new Neighbour($1, $2)
        [Test]
        public void _01_Pathfinder_Builds_Node_Map_Correctly() {
            Assert.That(_nodes.Keys.Count, Is.EqualTo(10));

            CheckNode(new SlidingPuzzleState(new Vector2Int(1, 4)), 
                new Neighbour(_up, new SlidingPuzzleState(new Vector2Int(1, 3))),
                new Neighbour(_right, new SlidingPuzzleState(new Vector2Int(3, 4))));

            CheckNode(new SlidingPuzzleState(new Vector2Int(1, 3)),
                new Neighbour(_down, new SlidingPuzzleState(new Vector2Int(1, 4))),
                new Neighbour(_right, new SlidingPuzzleState(new Vector2Int(4, 3))));

            CheckNode(new SlidingPuzzleState(new Vector2Int(1, 1)),
                new Neighbour(_right, new SlidingPuzzleState(new Vector2Int(3, 1))));

            CheckNode(new SlidingPuzzleState(new Vector2Int(2, 2)),
                new Neighbour(_right, new SlidingPuzzleState(new Vector2Int(4, 2))),
                new Neighbour(_down, new SlidingPuzzleState(new Vector2Int(2, 5), SlidingPuzzleState.StateType.Win)),
                new Neighbour(_up, new SlidingPuzzleState(new Vector2Int(2, 1))));

            CheckNode(new SlidingPuzzleState(new Vector2Int(2, 1)),
                new Neighbour(_left, new SlidingPuzzleState(new Vector2Int(1, 1))),
                new Neighbour(_down, new SlidingPuzzleState(new Vector2Int(2, 5), SlidingPuzzleState.StateType.Win)),
                new Neighbour(_right, new SlidingPuzzleState(new Vector2Int(3, 1))));

            CheckNode(new SlidingPuzzleState(new Vector2Int(3, 4)),
                new Neighbour(_left, new SlidingPuzzleState(new Vector2Int(1, 4))),
                new Neighbour(_up, new SlidingPuzzleState(new Vector2Int(3, 1))));

            CheckNode(new SlidingPuzzleState(new Vector2Int(3, 1)),
                new Neighbour(_left, new SlidingPuzzleState(new Vector2Int(1, 1))),
                new Neighbour(_down, new SlidingPuzzleState(new Vector2Int(3, 4))));

            CheckNode(new SlidingPuzzleState(new Vector2Int(4, 3)),
                new Neighbour(_left, new SlidingPuzzleState(new Vector2Int(1, 3))),
                new Neighbour(_up, new SlidingPuzzleState(new Vector2Int(4, 2))));

            CheckNode(new SlidingPuzzleState(new Vector2Int(4, 2)),
                new Neighbour(_left, new SlidingPuzzleState(new Vector2Int(2, 2))),
                new Neighbour(_down, new SlidingPuzzleState(new Vector2Int(4, 3))));

            Assert.That(_nodes, 
                Contains.Key(
                    new SlidingPuzzleState(new Vector2Int(2, 5), 
                        SlidingPuzzleState.StateType.Win)),
                "Nodes does not contain (2, 5) Winning state");

            void CheckNode(SlidingPuzzleState key, params Neighbour[] neighbours) {
                Assert.That(_nodes, Contains.Key(key), $"Nodes does not contain {key}");
                Assert.That(
                    _nodes[key].Neighbours.OrderBy(n => n.State.GetHashCode()).ToArray(),
                    Is.EqualTo(neighbours.OrderBy(n => n.State.GetHashCode()).ToArray()),
                    $"Incorrect neighbours for {key}");
            }
        }

        [Test]
        public void _02_Pathfinder_Find_Shortest_Path() {
            var path = _pathfinder.GetPath(_nodes, new SlidingPuzzleState(new Vector2Int(1, 4)));
            Assert.That(path,
                Is.EqualTo(new Vector2Int[] { _up, _right, _up, _left, _down }),
                "Resulting path is incorrect");
        }
    } 
}
