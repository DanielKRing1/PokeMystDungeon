using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class HitboxUtils
{
    public static void GetInSphereHitbox(Vector3 origin, float radius, Action<Collider> cb)
    {
        List<Collider> cols = Physics
            .OverlapSphere(origin, radius)
            .ToList();

        foreach(Collider col in cols) {
            cb(col);
        }
    }

    public static bool GetEnemiesInSphereHitbox(
        out List<GameObject> list,
        Vector3 origin,
        float radius,
        LeadershipManager myRootLeader
    ) {
        List<GameObject> temp = new List<GameObject>();

        GetInSphereHitbox(
            origin,
            radius,
            (Collider col) => {
                // Add if Alive and Different RootLeader
                if(col.gameObject.GetComponent<Brain>() != null && myRootLeader != col.gameObject.GetComponent<LeadershipManager>().GetRootLeader())
                    temp.Add(col.gameObject);
                // Else don't add
            }
        );
        
        list = temp;

        Debug.Log(list);
        return list.Count > 0;
    }

    public static void GetInSphereCastHitbox(
        Vector3 origin,
        float radius,
        Vector3 direction,
        float distance,
        Action<Collider> cb
    )
    {
        List<RaycastHit> hits = Physics
            .SphereCastAll(origin, radius, direction, distance)
            .ToList();

        foreach(RaycastHit hit in hits) {
            cb(hit.collider);
        }
    }

    public static bool GetEnemiesInSphereCastHitbox(
        out List<GameObject> list,
        Vector3 origin,
        float radius,
        Vector3 direction,
        float distance,
        LeadershipManager myRootLeader
    ) {
        List<GameObject> temp = new List<GameObject>();

        GetInSphereCastHitbox(
            origin,
            radius,
            direction,
            distance,
            (Collider col) => {
                // Alive and Different RootLeader
                if(col.gameObject.GetComponent<Brain>() != null && myRootLeader != col.gameObject.GetComponent<LeadershipManager>().GetRootLeader())
                    temp.Add(col.gameObject);
            }
        );

        list = temp;
        return list.Count > 0;
    }
}
