using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : AttrManager, LevelUpSensitive
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

        for (int i = 1; i < level; i++)
        {
            this.OnLevelUp();
        }
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
