using System;

public class PlayerBrain : Brain {

    protected override Type GetAttackBehaviorType()
    {
        return typeof(SpikeAttack);
    }

    protected override PubSub<LeadershipManager> GetInitLeader()
    {
        return null;
    }

    protected override int GetInitLeadership()
    {
        return LeadershipManager.MAX_LEADERSHIP;
    }

    protected override int GetInitLevel()
    {
        return 1;
    }

    protected override Stats GetInitStats()
    {
        return Stats.GenBaseStats();
    }

    protected override Type GetMoveBehaviorType()
    {
        return typeof(PlayerMove);
    }
}
