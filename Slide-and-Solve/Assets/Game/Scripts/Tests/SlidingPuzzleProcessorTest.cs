using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wokarol.PuzzleProcessors.Tests
{


    public class SlidingPuzzleProcessorTest
    {
        [Test]
        public void _01_Processor_Move_Correctly_Resolved() {
            var map = new SlidingPuzzleMap(
                new bool[3, 5] {
                    {true, true, true, true, true},
                    {true, false, false, false, true},
                    {true, true, true, true, true}
                }, new Vector2Int(1, 3));
            var processor = new SlidingPuzzleProcessor(map);
            var initState = new SlidingPuzzleState(new Vector2Int(1, 1));

            var result = processor.Process(initState, new Vector2Int(0, 1));

            Assert.That(result.State.PlayerCoords, Is.EqualTo(new Vector2Int(1, 3)));
            Assert.That(result.State.Type, Is.EqualTo(SlidingPuzzleState.StateType.Win));
        }
    }
}
