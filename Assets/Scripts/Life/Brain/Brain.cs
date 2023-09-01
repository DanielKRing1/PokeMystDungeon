using System;
using UnityEngine;

public abstract class Brain : MonoBehaviour
{
    private Stopwatch ifSW;
    private Stopwatch sSW;
    private Stopwatch bSW;

    private Observations observations = new Observations();

    protected virtual void Awake()
    {
        this.ifSW = new Stopwatch();
        this.sSW = new Stopwatch();
        this.bSW = new Stopwatch();

        this.Init();
    }

    protected void Init()
    {
        // AttributeManager's
        this.AddAttributeManagers();
        this.InitAttributeManagers();

        // PartOfLife's
        this.AddPartsOfLife();

        // Additional
        this.AddDestroyManager();
        this.AddIndicatorManager();
    }

    // ATTRIBUTE MANAGER COMPONENTS

    private void AddAttributeManagers()
    {
        this.gameObject.AddComponent<StatsManager>();
        this.gameObject.AddComponent<LeadershipManager>();
        this.gameObject.AddComponent<LevelManager>();
        this.gameObject.AddComponent<StatsManager>();
    }

    private void InitAttributeManagers()
    {
        // Random level
        int level = this.GetInitLevel();
        // Random Stats
        Stats stats = this.GetInitStats();
        //  Leadership lvl
        int leadership = this.GetInitLeadership();
        // Leader
        PubSub<LeadershipManager> leader = this.GetInitLeader();

        AttrManager[] mms = this.GetComponents<AttrManager>();
        foreach (AttrManager mm in mms)
        {
            mm.Init(level, stats, leadership, leader);
        }
    }

    protected abstract int GetInitLevel();
    protected abstract Stats GetInitStats();
    protected abstract int GetInitLeadership();
    protected abstract PubSub<LeadershipManager> GetInitLeader();

    // PART OF LIFE COMPONENTS

    private void AddPartsOfLife()
    {
        this.gameObject.AddComponent(this.GetAttackBehaviorType());
        this.gameObject.AddComponent(this.GetMoveBehaviorType());
        this.gameObject.AddComponent<HealthFunction>();
        this.gameObject.AddComponent<SightSense>();
    }

    protected abstract Type GetAttackBehaviorType();

    protected abstract Type GetMoveBehaviorType();

    // ADDITIONAL COMPONENTS

    private void AddDestroyManager()
    {
        this.gameObject.AddComponent<DestroyManager>();
    }

    /**
    This must be called after adding and initing AttributeManagers (specifically LeadershipManager),
    so that the VisualsManager has access to the RootLeader's color
    */
    private void AddIndicatorManager()
    {
        this.gameObject.AddComponent<VisualsManager>();
    }

    // EXECUTION

    public void Update()
    {
        this.Execute();
    }

    protected void Execute()
    {
        ExecutePartsOfLife<InternalFunction>(this.ifSW, 0.25f);
        ExecutePartsOfLife<Sense>(this.sSW, 0.25f);
        ExecutePartsOfLife<Behavior>(this.bSW, 0.25f);
    }

    private void ExecutePartsOfLife<T>(Stopwatch sw, float cooldown)
        where T : PartOfLife
    {
        if (!sw.HasElapsedStart(cooldown))
            return;

        T[] pols = this.GetComponents<T>();

        foreach (T pol in pols)
        {
            pol.Execute(this.observations);
        }

        sw.Start();
    }

    // UTILS

    public void ClearObservations()
    {
        this.observations = new Observations();
    }
}
