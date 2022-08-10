using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    const int TRAVERSAL_MULTIPLIER = 10;

    public Tile tile;
    public List<Prototype> prototypes;

    public Cell(Tile tile, List<Prototype> prototypes)
    {
        this.tile = tile;
        this.prototypes = new List<Prototype>(prototypes);
    }

    public bool IsCollapsed => prototypes.Count == 1;
    public int Entropy => prototypes.Count;

    public void Collapse()
    {
        //Prototype proto = prototypes[UnityEngine.Random.Range(0, prototypes.Count)];
        //prototypes = new List<Prototype>();
        //prototypes.Add(proto);

        int sumOfWeights = 0;
        foreach (Prototype p in prototypes)
        {
            sumOfWeights += (1 + (p.traversalScore * TRAVERSAL_MULTIPLIER));
        }
        int target = UnityEngine.Random.Range(0, sumOfWeights);
        int currentValue = 0;
        foreach (Prototype p in prototypes)
        {
            currentValue += 1 + (p.traversalScore * TRAVERSAL_MULTIPLIER);
            if (currentValue > target)
            {
                prototypes = new List<Prototype>();
                prototypes.Add(p);
                return;
            }
        }

        List<Prototype> candidates = GetHighestTraversalScorePrototypes();
        Prototype selected = candidates[UnityEngine.Random.Range(0, candidates.Count)];
        prototypes = new List<Prototype> { selected };
    }
    List<Prototype> GetHighestTraversalScorePrototypes()
    {
        List<Prototype> candidates = new List<Prototype> { prototypes[0] };
        int highestTraversalScore = prototypes[0].traversalScore;

        foreach (Prototype prototype in prototypes)
        {
            if (prototype == prototypes[0])
                continue;
            if (prototype.traversalScore > highestTraversalScore)
            {
                highestTraversalScore = prototype.traversalScore;
                candidates.Clear();
                candidates.Add(prototype);
                continue;
            }
            if (prototype.traversalScore == highestTraversalScore)
                candidates.Add(prototype);
        }

        return candidates;
    }

    public void CollapseTo(string prototypeName)
    {
        Prototype proto = prototypes.Find(p => p.name == prototypeName);
        prototypes = new List<Prototype>();
        prototypes.Add(proto);
    }

    public void Constrain(Prototype prototype)
    {
        prototypes.Remove(prototype);
    }
}
