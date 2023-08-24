using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    private Stopwatch ifSW;
    private Stopwatch sSW;
    private Stopwatch bSW;

    private Observations observations = new Observations();

    protected virtual void Start()
    {
        // Random level
        int level = (int)Random.Range(1, 10);
        // Random Stats
        Stats baseStats = Stats.GenBaseStats();
        // Random Leadership lvl
        int leadership = (int)
            Random.Range(LeadershipManager.MIN_LEADERSHIP, LeadershipManager.MAX_LEADERSHIP);
        PubSub<LeadershipManager> leader = null;

        this.Init(level, baseStats, leadership, leader);
    }

    protected void Init(
        int level,
        Stats stats,
        int leadership = -1,
        PubSub<LeadershipManager> leader = null
    )
    {
        this.ifSW = new Stopwatch();
        this.sSW = new Stopwatch();
        this.bSW = new Stopwatch();

        AttrManager[] mms = this.GetComponents<AttrManager>();

        foreach (AttrManager mm in mms)
        {
            mm.Init(level, stats, leadership, leader);
        }
    }

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
        if (!sw.HasElapsed(cooldown))
            return;

        T[] pols = this.GetComponents<T>();

        foreach (T pol in pols)
        {
            pol.Execute(this.observations);
        }

        sw.Start();
    }

    public void ClearObservations()
    {
        this.observations = new Observations();
    }
}
