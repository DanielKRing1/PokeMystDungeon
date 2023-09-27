using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBrain : Brain
{
    // FOR ATTRIBUTE MANAGER COMPONENTS

    protected override PubSub<LeadershipManager> GetInitLeader()
    {
        return null;
    }

    protected override int GetInitLeadership()
    {
        // Random Leadership lvl
        return Random.Range(LeadershipManager.MIN_LEADERSHIP, LeadershipManager.MAX_LEADERSHIP);
    }

    protected override int GetInitLevel()
    {
        // Random level
        return RandUtils.GetRandInt(1, 10);
    }

    protected override Stats GetInitStats()
    {
        // Random Stats
        return Stats.GenBaseStats();
    }

    // FOR PART OF LIFE COMPONENTS

    protected override System.Type GetAttackBehaviorType()
    {
        return AttackBehavior.ATTACK_BEHAVIOR_TYPES[3];
        // return RandUtils.GetRandListElement(AttackBehavior.ATTACK_BEHAVIOR_TYPES);
    }

    protected override System.Type GetMoveBehaviorType()
    {
        return typeof(MoveBehavior);
    }
}
