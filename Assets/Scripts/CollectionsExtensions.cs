using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CollectionsExtensions
{
    public static T GetRandom<T>(this List<T> source)
    {
        if (source.Count == 0)
            return default;

        return source[Random.Range(0, source.Count)];
    }
    
    public static string ToDisplayString<T>(this IEnumerable<T> source)
    {
        if (source == null)
            return "null";

        string s = "[";
        s = source.Aggregate(s, (res, x) => res + x + ", ");

        if (s.Contains(", "))
            s = s.Substring(0, s.Length - 2);

        return $"{s}]";
    }
}