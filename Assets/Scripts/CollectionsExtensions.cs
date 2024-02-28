using System.Collections.Generic;
using UnityEngine;

public static class CollectionsExtensions
{
    public static T GetRandom<T>(this List<T> source)
    {
        if (source.Count == 0)
            return default;

        return source[Random.Range(0, source.Count)];
    }
}