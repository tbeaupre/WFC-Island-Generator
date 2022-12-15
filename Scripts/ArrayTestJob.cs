using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct ArrayTestJob : IJobParallelFor
{
    public struct Input
    {
        public int ints;
    }

    public struct Output
    {
        public int ints;
    }

    [ReadOnly]
    public NativeArray<Input> inputs;
    public NativeArray<Output> outputs;

    void IJobParallelFor.Execute(int i)
    {
        Output output = new Output();
        output.ints = inputs[i].ints;
        outputs[i] = output;
    }
}
