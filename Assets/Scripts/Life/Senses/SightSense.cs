using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class SightSense : Sense
{

    private CircleLineRenderer visionOutline;

    public void Start() {
        this.visionOutline = new CircleLineRenderer(20);
        this.visionOutline.Draw(this.gameObject, this.GetStats().Vision, this.GetComponent<LeadershipManager>().GetRootLeaderColor(), this.GetComponent<LeadershipManager>().GetRootLeaderColor());
    }

    public override void Execute(Observations obs)
    {
        // 1. Create new data containers
        List<List<GameObject>> nearbyEnemies = Enumerable
            .Range(0, LeadershipManager.MAX_LEADERSHIP)
            .Select(_ => new List<GameObject>())
            .ToList();

        List<GameObject> nearbyAllies = new List<GameObject>();

        // 2. Get nearby colliders
        Collider[] nearbyColliders = Physics.OverlapSphere(
            this.gameObject.transform.position,
            this.GetStats().Vision
        );

        LeadershipManager myLm = this.GetComponent<LeadershipManager>();
        foreach (var col in nearbyColliders)
        {
            // 3. Short-circuit if no Brain
            Brain otherBrain = col.GetComponent<Brain>();
            if (otherBrain == null)
                continue;

            // If ally
            LeadershipManager otherLm = col.GetComponent<LeadershipManager>();
            if (myLm.pubSub.GetRootPub() == otherLm.pubSub.GetRootPub())
            {
                nearbyAllies.Add(col.gameObject);
            }
            // If enemy
            else
            {
                // 5. Add to data container, depending on Leadership
                nearbyEnemies[otherLm.Leadership].Add(col.gameObject);
            }
        }

        // 6. Update Observations object
        obs.nearbyEnemies = nearbyEnemies;
        obs.nearbyAllies = nearbyAllies;
    }
}
