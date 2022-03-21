using UnityEngine;
using System.Collections.Generic;


static class Utils
{
    static public IList<T> MakeList<T>(this T obj)
    {
        return new List<T>() { obj };
    }

    static public T PickRandom<T>(this IList<T> list)
    {
        if (list.Count < 1)
            throw new System.Exception("List is empty!");

        if (list.Count == 1)
            return list[0];

        var i = Random.Range(0, list.Count);
        return list[i];
    }
}
