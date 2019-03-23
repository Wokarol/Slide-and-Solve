using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.PuzzleProcessors;

namespace Wokarol.Pathfinder
{
    public class PuzzlePathinder<MoveT, StateT>
    {
        public MoveT[] GetPath(IPuzzleProcessor<MoveT, StateT> processor, StateT startingState) {

            return new MoveT[0];
        }
    }
}
