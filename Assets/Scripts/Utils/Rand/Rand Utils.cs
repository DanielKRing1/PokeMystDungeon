using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandUtils
{
    public static T GetRandListElement<T>(List<T> list)
    {
        return list[GetRandInt(0, list.Count)];
    }

    public static int GetRandInt(int min, int max)
    {
        return (int)Mathf.Floor(Random.Range(min, max + 1));
    }
}
