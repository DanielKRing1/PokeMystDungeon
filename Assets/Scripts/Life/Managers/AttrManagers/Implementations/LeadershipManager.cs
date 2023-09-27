using System;
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

        this.GetComponent<VisualsManager>().ApplyRootLeaderColor();
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
    Recruits this Sub to a potential Pub and alters this Health or other attributes

    Returns false if fails to recruit, else true
    */
    public bool TryRecruitOnceDead(LeadershipPubSub potentialPub)
    {
        // 1. Not Dead
        if (!this.Me.GetComponent<HealthFunction>().IsDead())
            return false;

        // 2. Short-circuit if failed recruit criteria (if new leader does not have higher leadership level)
        LeadershipPubSub newPub = this.TryRecruitChallenge(potentialPub);
        return newPub != null;
    }

    /**
    Looks for first upstream Pub that has a high enough LeadershipLevel to recruit this as Sub,
    Then tries to recruit

    Returns LeadershipPubSub that can recruit, or null if none can
    */
    public LeadershipPubSub TryRecruitChallenge(LeadershipPubSub startingPub)
    {
        LeadershipPubSub newPub = null;

        try
        {
            LeadershipPubSub candidatePub = startingPub;
            bool isValidCandidate = !this.ValidatePub(candidatePub);
            while (candidatePub.HasPub() && !isValidCandidate)
            {
                candidatePub = (LeadershipPubSub)candidatePub.GetPub();
                isValidCandidate = this.ValidatePub(candidatePub);
            }

            // 1. All upstream Pubs failed requirements to be new Pub
            if (!isValidCandidate)
                return null;

            // 2. Luck might still not allow it
            if (RandUtils.GetRandFloat(0, 1) >= candidatePub.Me.GetStats().Luck)
                return null;

            // 3. Success, recruit
            newPub = candidatePub;
            this.AddPub(newPub);

            // 4. Get max Health
            float maxHealth = this.Me.GetStats().Health;

            // 5. Revive to max Health
            HealthFunction hf = this.Me.GetComponent<HealthFunction>();
            hf.Heal((int)maxHealth);

            // 6. Reset Observations
            Brain brain = this.Me.GetComponent<Brain>();
            brain.ClearObservations();

            // 7. ApplyRootLeaderColor
            this.Me.GetComponent<VisualsManager>().ApplyRootLeaderColor();

            return candidatePub;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        return newPub;
    }

    /**
    Criteria for recruiting this Sub

    Sub validates Pub
    */
    protected override bool ValidatePub(PubSub<LeadershipManager> pub)
    {
        // 1. I already have a Leader
        if (this.HasPub())
            return false;

        // 2. Pub Leadership must scale above my Leadership
        return pub.Me.Leadership > this.Me.Leadership;
    }

    /**
    Pub validates Sub
    */
    protected override bool ValidateSub(PubSub<LeadershipManager> sub)
    {
        return this.Me.Leadership > sub.Me.Leadership;
    }
}
