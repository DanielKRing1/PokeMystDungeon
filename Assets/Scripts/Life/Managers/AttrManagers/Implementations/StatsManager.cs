using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : AttrManager, ILevelUpSensitive
{
    private Stats Stats;

    public override void Init(
        int level,
        Stats stats,
        int leadership = -1,
        PubSub<LeadershipManager> leader = null
    )
    {
        this.Stats = stats;
    }

    public void OnLevelUp()
    {
        this.Stats.LevelUpStats();
    }

    public Stats GetStats()
    {
        return this.Stats;
    }
}
