using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentUtils
{
    public static bool Copy<T>(GameObject src, GameObject target)
    {
        try
        {
            T srcComponent = src.GetComponent<T>();
            var newComponent = target.AddComponent(srcComponent.GetType());
            Debug.Log(newComponent);

            return true;
        }
        catch (System.Exception e)
        {
            Debug.Log(e);

            return false;
        }
    }
}
