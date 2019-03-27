using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.PuzzleProcessors;

namespace Wokarol.IntegartionTests
{
    public class SlidingPuzzlePathfindingTest
    {
        readonly Vector2Int _down = new Vector2Int(0, -1);
        readonly Vector2Int _up = new Vector2Int(0, 1);
        readonly Vector2Int _right = new Vector2Int(1, 0);
        readonly Vector2Int _left = new Vector2Int(-1, 0);

        [Test]
        public void _01_Pathfinder_Solves_The_Puzzle() {
            SlidingPuzzleMap map = new SlidingPuzzleMap(
                "111111;" +
                "100011;" +
                "110001;" +
                "100001;" +
                "100011;" +
                "11W111;" +
                "111111");
            SlidingPuzzleState startingState = new SlidingPuzzleState(new Vector2Int(1, 2));

            var processor = new SlidingPuzzleProcessor(map);
            var pathfinder = new Pathfinder.PuzzlePathfinder<Vector2Int, SlidingPuzzleState>().RecalculateGrah(processor, startingState, new Vector2Int[] { _up, _down, _left, _right });



            Assert.That(pathfinder.Graph.Count, Is.EqualTo(10), $"Incorect graph size");
            Assert.That(pathfinder.StatesToGraphMap.Count, Is.EqualTo(10), $"Incorect graph map size");

            Assert.That(pathfinder.StatesToGraphMap, Contains.Key(startingState));

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            var path = pathfinder.GetClosestWinPath(startingState);
            stopwatch.Stop();

            Debug.Log($"Pathfinding had taken: {stopwatch.ElapsedMilliseconds} ms [{stopwatch.ElapsedTicks} ticks]");

            Assert.That(path.Succesfull, Is.EqualTo(true), $"Path is not marked as succesful");
            Assert.That(path.Path, Is.EqualTo(new Vector2Int[] {_up, _right, _up, _left, _down}), $"Path is incorrect");
        }

        [Test]
        public void _02_Pathfinder_Solves_The_Puzzle() {
            SlidingPuzzleMap map = new SlidingPuzzleMap(
                "11111111;" +
                "11000101;" +
                "11000101;" +
                "10000101;" +
                "10000001;" +
                "10001111;" +
                "11000001;" +
                "10000001;" +
                "11W11111;" +
                "11111111");
            SlidingPuzzleState startingState = new SlidingPuzzleState(new Vector2Int(1, 6));

            var processor = new SlidingPuzzleProcessor(map);
            var pathfinder = new Pathfinder.PuzzlePathfinder<Vector2Int, SlidingPuzzleState>().RecalculateGrah(processor, startingState, new Vector2Int[] { _up, _down, _left, _right });

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            var path = pathfinder.GetClosestWinPath(startingState);
            stopwatch.Stop();

            Debug.Log($"Pathfinding had taken: {stopwatch.ElapsedMilliseconds} ms [{stopwatch.ElapsedTicks} ticks]");

            Assert.That(path.Succesfull, Is.EqualTo(true), $"Path is not marked as succesful");
            Assert.That(path.Path, Is.EqualTo(new Vector2Int[] { _right, _up, _left, _down}), $"Path is incorrect");
        }
    } 
}
