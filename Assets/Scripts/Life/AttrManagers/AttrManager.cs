using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttrManager : MonoBehaviour
{
    public Stats GetStats()
    {
        return this.GetComponent<StatsManager>().GetStats();
    }

    public abstract void Init(
        int level,
        Stats stats,
        int leadership = -1,
        PubSub<LeadershipManager> leader = null
    );
}
