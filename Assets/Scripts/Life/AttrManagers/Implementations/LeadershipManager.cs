using System.Collections.Generic;
using UnityEngine;

public class LeadershipManager : AttrManager
{
    internal const int MIN_LEADERSHIP = 0;
    internal const int MAX_LEADERSHIP = 4;

    public Color LeadershipColor;

    public LeadershipPubSub pubSub;

    private int _leadership;
    public int Leadership
    {
        get { return _leadership; }
        private set { _leadership = value; }
    }

    public override void Init(
        int level,
        Stats stats,
        int leadership = -1,
        PubSub<LeadershipManager> leader = null
    )
    {
        this.LeadershipColor = RandomColor.GetRandColor();

        this.pubSub = new LeadershipPubSub(this, leader);

        this.Leadership =
            leadership > -1
                ? leadership
                : UnityEngine.Random.Range(
                    LeadershipManager.MIN_LEADERSHIP,
                    LeadershipManager.MAX_LEADERSHIP + 1
                );
    }

    public LeadershipManager GetRootLeader()
    {
        return this.pubSub.GetRootPub().Me;
    }

    public Color GetRootLeaderColor()
    {
        return this.GetRootLeader().LeadershipColor;
    }
}

public class LeadershipPubSub : PubSub<LeadershipManager>
{
    public LeadershipPubSub(LeadershipManager me, PubSub<LeadershipManager> pub)
        : base(me, pub)
    {
        //other stuff here
    }

    // RECRUIT

    // In

    /**
    Recruits (sets the Leader of) this Entity and alters its Health or other attributes

    Returns false if fails to recruit, else true
    */
    public bool TryRecruitDeadSub(LeadershipPubSub newPub)
    {
        // 0.1. I already have a Leader
        if (this.HasPub())
            return false;

        // 0.2. Luck might not allow it
        if (
            UnityEngine.Random.Range(0, 1) >= newPub.Me.GetComponent<StatsManager>().GetStats().Luck
        )
            return false;

        // 1. Short-circuit if failed to recruit (if new leader does not have higher leadership level)
        if (!this.AddPub(newPub))
            return false;

        // 2. Get max Health
        float maxHealth = this.Me.GetStats().Health;

        // 3. Revive to max Health
        HealthFunction hf = this.Me.GetComponent<HealthFunction>();
        hf.Heal((int)maxHealth);

        // 4. Reset Observations
        Brain brain = this.Me.GetComponent<Brain>();
        brain.ClearObservations();

        return true;
    }

    protected override bool ValidatePub(PubSub<LeadershipManager> pub)
    {
        HashSet<PubSub<LeadershipManager>> allPubs = new HashSet<PubSub<LeadershipManager>>();
        this.GetAllPubs(allPubs);

        // Pub is not already an upstream Pub and Pub Leadership must scale above my Leadership
        return !allPubs.Contains(pub) && pub.Me.Leadership > this.Me.Leadership;
    }

    protected override bool ValidateSub(PubSub<LeadershipManager> sub)
    {
        HashSet<PubSub<LeadershipManager>> allSubs = new HashSet<PubSub<LeadershipManager>>();
        this.GetAllSubs(allSubs);

        // Sub is not already a downstream Sub and My Leadership must scale above Sub Leadership
        return !allSubs.Contains(sub) && this.Me.Leadership > sub.Me.Leadership;
    }
}
