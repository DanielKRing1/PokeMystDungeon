using UnityEngine;

public class LeadershipManager : AttrManager
{
    internal const int MIN_LEADERSHIP = 0;
    internal const int MAX_LEADERSHIP = 4;

    public Color LeadershipColor;
    private CircleSpriteRenderer LeaderIndicator;

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
        this.LeaderIndicator = new CircleSpriteRenderer(this.gameObject);

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

    public void ApplyLeaderIndicatorColor()
    {
        Color color = this.GetRootLeaderColor();

        this.LeaderIndicator.Draw(1, color);
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
        // 0.1. If I have no Leader
        if (this.HasPub())
            return false;

        // 0.2. If Luck allows it
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
        // New potential leader does not scale above existing leader
        return pub.Me.Leadership >= this.GetPub().Me.Leadership;
    }

    protected override bool ValidateSub(PubSub<LeadershipManager> sub)
    {
        // New potential leader does not scale above existing leader
        return this.Me.Leadership >= sub.Me.Leadership;
    }
}
