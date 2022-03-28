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

    static public string FormatNumber(int n)
    {
        var text = n.ToString();
        if (text.Length == 0)
        {
            var zeros = new string('0', 8);
            text = "<color=#E7002F>" + zeros;
        }
        else if (text.Length < 8)
        {
            var zeros = new string('0', 8 - text.Length);
            text = "<color=#E7002F>" + zeros + "<color=white>" + text;
        }
        return text;
    }
}
