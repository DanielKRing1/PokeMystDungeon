using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class SightSense : Sense
{
    public override void Execute(Observations obs)
    {
        // 1. Create new data containers
        List<List<GameObject>> nearbyEnemies = Enumerable
            .Range(0, LeadershipManager.MAX_LEADERSHIP + 1)
            .Select(_ => new List<GameObject>())
            .ToList();

        List<GameObject> nearbyAllies = new List<GameObject>();

        // 2. Get nearby colliders
        LeadershipManager myLm = this.GetComponent<LeadershipManager>();
        HitboxUtils.GetInSphereHitbox(
            this.gameObject.transform.position,
            this.GetStats().Vision,
            (Collider col) =>
            {
                // 3. Short-circuit if no Brain
                Brain otherBrain = col.GetComponent<Brain>();
                if (otherBrain == null)
                    return;

                // If ally
                LeadershipManager otherLm = col.GetComponent<LeadershipManager>();
                if (myLm.GetRootLeader() == otherLm.GetRootLeader())
                {
                    // 4. Add to ally data container
                    nearbyAllies.Add(col.gameObject);
                }
                // If enemy
                else
                {
                    // 5. Add to enemy data container, depending on Leadership
                    nearbyEnemies[otherLm.Leadership].Add(col.gameObject);
                }
            }
        );

        // 6. Update Observations object
        obs.nearbyEnemies = nearbyEnemies;
        obs.nearbyAllies = nearbyAllies;
    }
}
