using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VectorService
{
    public static Vector3 CalculateAverageVector(List<Vector3> vectors)
    {
        return vectors.Any() ? vectors.Aggregate(Vector3.zero, (current, vector) => current + vector) / vectors.Count : Vector3.zero;
    }
}
