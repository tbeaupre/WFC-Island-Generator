using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct PropagateJob : IJobParallelFor
{
    public struct PropagateInput
    {
        public Direction dir;
        public Cell cell;
        public List<Prototype> neighborPrototypes;
    }

    public struct PropagateOutput
    {
        public bool wasSuccessful;
        public bool shouldPropagateToNeighbor;
        public List<Prototype> neighborPrototypes;

        public PropagateOutput(List<Prototype> neighborPrototypes)
        {
            this.wasSuccessful = false;
            this.shouldPropagateToNeighbor = false;
            this.neighborPrototypes = neighborPrototypes;
        }
    }

    [ReadOnly]
    public NativeArray<PropagateInput> input;
    public NativeArray<PropagateOutput> output;

    public void Execute(int i)
    {
        PropagateInput data = input[i];
        PropagateOutput result = new PropagateOutput(data.neighborPrototypes);

        if (data.neighborPrototypes.Count == 0)
        {
            output[i] = result; // by default wasSuccessful is false
            return;
        }

        HashSet<string> possibleNeighbors = GetPossibleNeighbors(data.cell, data.dir);

        foreach (Prototype otherPrototype in data.neighborPrototypes)
        {
            if (!possibleNeighbors.Contains(otherPrototype.name))
            {
                result.neighborPrototypes.Remove(otherPrototype);
                if (result.neighborPrototypes.Count == 0)
                {
                    output[i] = result; // by default wasSuccessful is false
                    return;
                }
                result.shouldPropagateToNeighbor = true;
            }
        }

        result.wasSuccessful = true;
        output[i] = result;
    }

    private HashSet<string> GetPossibleNeighbors(Cell cell, Direction dir)
    {
        HashSet<string> possibleNeighbors = new HashSet<string>();
        foreach (Prototype p in cell.prototypes)
        {
            switch (dir)
            {
                case Direction.Back:
                    possibleNeighbors.UnionWith(p.validNeighbors.back);
                    break;
                case Direction.Right:
                    possibleNeighbors.UnionWith(p.validNeighbors.right);
                    break;
                case Direction.Left:
                    possibleNeighbors.UnionWith(p.validNeighbors.left);
                    break;
                case Direction.Top:
                    possibleNeighbors.UnionWith(p.validNeighbors.top);
                    break;
                case Direction.Bottom:
                    possibleNeighbors.UnionWith(p.validNeighbors.bottom);
                    break;
            }
        }
        return possibleNeighbors;
    }
}
