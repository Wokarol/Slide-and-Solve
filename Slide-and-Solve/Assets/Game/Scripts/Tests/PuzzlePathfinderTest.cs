using NUnit.Framework;
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


        private PuzzlePathinder<int, char> _pathfinder;
        private IPuzzleProcessor<int, char> _processor;

        [SetUp]
        public void Setup() {
            _pathfinder = new PuzzlePathinder<int, char>();
            _processor = new FakePuzzleProcessor();
        }

        [Test]
        public void _00_Setted_Up_Correctly() {
            Assert.That(_processor.Process('D', 2), Is.EqualTo('C'), $"Fake processor has incorrect map");
            Assert.That(_processor.Process('E', 3), Is.EqualTo('E'), $"Fake processor has incorrect map");
            Assert.That(_processor.Process('A', 1), Is.EqualTo('B'), $"Fake processor has incorrect map");
        }

        [Test]
        public void _01_Pathfinder_Find_Shortest_Path() {

        }
    }

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
    }
}
