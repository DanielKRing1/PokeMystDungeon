using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomColor
{
    public static Color GetRandColor()
    {
        return new Color(Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1));
    }
}
