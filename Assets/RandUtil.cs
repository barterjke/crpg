using System.Collections.Generic;
using System;

public static class RandUtil
{
    private static Random rand = new Random(DateTime.Now.ToString().GetHashCode());

    public static T PickRandom<T>(this T[] array)
    {
        if (array == null || array.Length == 0)
            throw new ArgumentException("Array is empty or null", nameof(array));
        int randomIndex = rand.Next(array.Length);
        return array[randomIndex];
    }

    public static T PickRandom<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
            throw new ArgumentException("List is empty or null", nameof(list));
        int randomIndex = rand.Next(list.Count);
        return list[randomIndex];
    }
}