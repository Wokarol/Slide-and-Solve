﻿using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.PuzzleProcessors;
using System.Linq;

namespace Wokarol.Pathfinder
{
    public class PuzzlePathfinderTest
    {


        private PuzzlePathfinder<int, char> _pathfinder;
        private IPuzzleProcessor<int, char> _processor;

        [SetUp]
        public void Setup() {
            _processor = new FakePuzzleProcessor();
            _pathfinder = new PuzzlePathfinder<int, char>().RecalculateGrah(_processor, 'A', new int[] { 1, 2, 3, 4 });
        }

        [Test]
        public void _00_Setted_Up_Correctly() {
            // Random Tests
            Assert.That(_processor.Process('D', 2), Is.EqualTo('C'), $"Fake processor has incorrect map");
            Assert.That(_processor.Process('E', 3), Is.EqualTo('E'), $"Fake processor has incorrect map");
            Assert.That(_processor.Process('A', 1), Is.EqualTo('B'), $"Fake processor has incorrect map");
        }

        [Test]
        public void _01_Pathfinder_Correctly_Calculates_Graph() {
            var graph = _pathfinder.Graph;
            var map = _pathfinder.StatesToGraphMap;
            Assert.That(graph.Count, Is.EqualTo(6), $"Incorect Graph size");
            Assert.That(map.Keys.Count, Is.EqualTo(6), $"Incorect Graph Map size");

            // Random Tests
            Assert.That(map['C'].Neighbours.Count, Is.EqualTo(3), $"C has incorrect number of neighbours");
            Assert.That(map['E'].Neighbours, Contains.Item(new PuzzlePathfinder<int, char>.Node.Neighbour(map['F'], 2)), $"E doesn't contain F as neighbour or it is not accessible by move \"2\"");
            Assert.That(map['B'].Neighbours, Is.Empty, $"B's neighbours aren't empty");
        }

        [TestCase('A', 'E', true, 3, 3)]
        [TestCase('D', 'B', true, 2, 1, 1)]
        [TestCase('B', 'E', false)]
        [TestCase('C', 'C', true)]
        public void _02_Pathfinder_Finds_Shortest_Path(char start, char end, bool expectedSuccesfull, params int[] expectedPath) {
            var result = _pathfinder.GetPath(start, end);
            Assert.That(result.Path, Is.EqualTo(expectedPath), $"Path from {start} to {end} is incorrect");
            Assert.That(result.Succesfull, Is.EqualTo(expectedSuccesfull), $"Path is not {(expectedSuccesfull ? "Succesfull" : "Unsuccesfull")}");
        }

        [TestCase('A', 1)]
        [TestCase('F', 1, 3)]
        [TestCase('D', 2, 3)]
        public void _03_Pathfinder_Finds_Shortest_Path_To_Win(char start, params int[] expectedPath) {
            var result = _pathfinder.GetClosestWinPath(start);
            Assert.That(result.Path, Is.EqualTo(expectedPath), $"Path from {start} to win is incorrect");
        }
    }

    /// <summary>
    /// Puzzle Processor based on this graph: https://imgur.com/SAQh0Qj
    /// </summary>
    internal class FakePuzzleProcessor : IPuzzleProcessor<int, char>
    {
        Dictionary<char, Dictionary<int, char>> map = new Dictionary<char, Dictionary<int, char>> {
            {'A', new Dictionary<int, char> { { 1, 'B'}, { 2, 'D'}, { 3, 'C'} } },
            {'B', new Dictionary<int, char>() },
            {'C', new Dictionary<int, char> { { 1, 'A'}, { 2, 'F'}, { 3, 'E'} } },
            {'D', new Dictionary<int, char> { { 1, 'F'}, { 2, 'C'} } },
            {'E', new Dictionary<int, char> { { 1, 'A'}, { 2, 'F'} } },
            {'F', new Dictionary<int, char> { { 1, 'C'} } }
        };

        public char Process(char state, int move) {
            var roads = map[state];
            if (roads.ContainsKey(move))
                return roads[move];
            return state;
        }

        public bool StateIsWinning(char state) {
            return state == 'E' || state == 'B';
        }
    }
}
