using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Wokarol.PuzzleProcessors.SlidingPuzzleProcessor;

namespace Wokarol.PuzzleProcessors
{
    public class SlidingPuzzleProcessorTest
    {
        [Test]
        public void _01_String_Parsing_Works() {
            var walls = new bool[4, 5] {
                    {true, true, true, true, true },
                    {true, false, false, false, true },
                    {true, true, true, false, true },
                    {true, true, true, true, true }
                };

            var map = new SlidingPuzzleMap(
                "1111;" +
                "1001;" +
                "1011;" +
                "1W1;" +
                "111");

            Assert.That(new Vector2Int(map.Walls.GetLength(0), map.Walls.GetLength(1)), Is.EqualTo(new Vector2Int(4, 5)), "Size in incorrect");
            Assert.That(map.Walls, Is.EqualTo(walls), $"Incorrect array\nExpected:\n{ArrayToText(walls)}and got:\n{ArrayToText(map.Walls)}");
            Assert.That(map.WinCoords, Is.EqualTo(new Vector2Int(1, 1)), $"Win is in incorect place");
        }

        [Test]
        public void _02_String_Parsing_Throws_Exception_When_There_Is_More_Than_One_Win() {

            Assert.That(
                () => new SlidingPuzzleMap(
                    "1111;" +
                    "10W1;" +
                    "1011;" +
                    "1W1;" +
                    "111"),
                Throws.ArgumentException.
                With.Message.EqualTo("There's more than one win on (1, 1) and (2, 3)"));

            Assert.That(
                () => new SlidingPuzzleMap(
                    "11111;" +
                    "1W001;" +
                    "110W1;" +
                    "11011;" +
                    "1111"),
                Throws.ArgumentException
                .With.Message.EqualTo("There's more than one win on (3, 2) and (1, 3)"));
        }

        [Test]
        public void _03_String_Parsing_Throws_Exception_When_There_Is_No_Win() {

            Assert.That(
                () => new SlidingPuzzleMap(
                    "1111;" +
                    "1001;" +
                    "1011;" +
                    "101;" +
                    "111"),
                Throws.ArgumentException.
                With.Message.EqualTo("There's no win"));
        }


        // quick regex: 2 1 -> 0 1 -> 2 1 false
        // (-?\d) (-?\d) -> (-?\d) (-?\d) -> (-?\d) (-?\d) (\w+)
        // [TestCase(new int[] { $1, $2 }, new int[] { $3, $4 }, new int[] { $5, $6 }, $7, TestName = "From ($1, $2) with ($3, $4) dir to ($5,$6)")]
        [TestCase(new int[] { 1, 3 }, new int[] { 0, -1 }, new int[] { 1, 1 }, true, TestName = "From (1, 3) with (0, -1) dir to (1, 1) and win")]
        [TestCase(new int[] { 1, 3 }, new int[] { 1, 0 }, new int[] { 2, 3 }, false, TestName = "From (1, 3) with (1, 0) dir to (2, 3)")]
        [TestCase(new int[] { 2, 3 }, new int[] { -1, 0 }, new int[] { 1, 3 }, false, TestName = "From (2, 3) with (-1, 0) dir to (1, 3)")]
        [TestCase(new int[] { 2, 3 }, new int[] { 0, -1 }, new int[] { 2, 3 }, false, TestName = "From (2, 3) with (0, -1) dir to (2, 3)")]
        public void _04_Processor_Move_Correctly_Resolved(int[] start, int[] dir, int[] expected, bool expectedWin) {
            var map = new SlidingPuzzleMap(
                "1111;" +
                "1001;" +
                "1011;" +
                "1W1;" +
                "111");
            var processor = new SlidingPuzzleProcessor(map);
            var initState = new SlidingPuzzleState(new Vector2Int(start[0], start[1]));

            var result = processor.Process(initState, new Vector2Int(dir[0], dir[1]));

            Assert.That(result.PlayerCoords, Is.EqualTo(new Vector2Int(expected[0], expected[1])), "Player is in incorrect position");
            if (expectedWin)
                Assert.That(result.Type, Is.EqualTo(SlidingPuzzleState.StateType.Win), "Move is not registered as winning");
            else
                Assert.That(result.Type, Is.EqualTo(SlidingPuzzleState.StateType.Idle), "Move is registered as winning");
        }

        [Test]
        public void _05_Processor_Calculates_Transitions_Correclty() {
            var map = new SlidingPuzzleMap(
                "1111;" +
                "10W1;" +
                "101;" +
                "101;" +
                "111");
            var processor = new SlidingPuzzleProcessor(map);
            var initState = new SlidingPuzzleState(new Vector2Int(1, 3));

            var result = processor.ProcessWithTransitions(initState, new Vector2Int(0, -1));

            Assert.That(result.Transitions.Length, Is.EqualTo(1), $"Transitions Size Incorrect");
            Assert.That(result.Transitions[0],
                Is.EqualTo(new Transition(initState.PlayerCoords, new Vector2Int(1, 1))),
                $"Transition list is incorrect");
        }

        string ArrayToText(bool[,] map) {
            string result = "";
            for (int y = map.GetLength(1) - 1; y >= 0; y--) {
                result += $"y - {y} -> [";
                for (int x = 0; x < map.GetLength(0); x++) {
                    result += map[x, y] ? "1" : "0";
                }
                result += "]\n";
            }
            return result;
        }
    }
}
