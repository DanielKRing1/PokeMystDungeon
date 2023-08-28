using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObservationsUtils
{
    public static List<GameObject> SortEnemiesByDistance(GameObject me, Observations obs)
    {
        // 1. Sort nearby enemy GameObjects
        List<GameObject> nearbySorted = new List<GameObject>();
        foreach (List<GameObject> nearbyStrata in obs.nearbyEnemies)
        {
            foreach (GameObject enemy in nearbyStrata)
            {
                nearbySorted.Add(enemy);
            }
        }
        nearbySorted.Sort(
            (a, b) =>
                Vector3.Distance(a.transform.position, me.transform.position)
                    - Vector3.Distance(b.transform.position, me.transform.position)
                >= 0
                    ? 1
                    : -1
        );

        return nearbySorted;
    }
}
