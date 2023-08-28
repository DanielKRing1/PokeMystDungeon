using UnityEngine;

public static class RandomColor
{
    public static Color GetRandColor()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }
}
