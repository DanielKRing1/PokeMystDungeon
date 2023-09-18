using System;
using UnityEngine;

public abstract class Brain : MonoBehaviour
{
    // NOTE: It is important to init the AttributeManagers in a certain order (or at least, the LevelManager should be initted AFTER the ILevelSensitve AttributeManagers)
    private Type[] ATTRIBUTE_MANAGERS = new Type[]
    {
        typeof(StatsManager),
        typeof(LeadershipManager),
        typeof(LevelManager)
    };

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
        // "External" Manager (Dont know what to call them atm)
        this.AddVisualsManager();
        this.AddDestroyManager();

        // AttributeManager's
        this.AddAttributeManagers();
        this.InitAttributeManagers();

        // PartOfLife's
        this.AddPartsOfLife();
    }

    // ATTRIBUTE MANAGER COMPONENTS

    private void AddAttributeManagers()
    {
        foreach (Type am in this.ATTRIBUTE_MANAGERS)
        {
            this.gameObject.AddComponent(am);
        }
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

        foreach (Type t in this.ATTRIBUTE_MANAGERS)
        {
            (this.GetComponent(t) as AttrManager).Init(level, stats, leadership, leader);
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
        DestroyManager dm = this.gameObject.AddComponent<DestroyManager>();
        dm.Init();
    }

    /**
    This must be called after adding and initing AttributeManagers (specifically LeadershipManager),
    so that the VisualsManager has access to the RootLeader's color
    */
    private void AddVisualsManager()
    {
        VisualsManager vm = this.gameObject.AddComponent<VisualsManager>();
        vm.Init();
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
    }

    // UTILS

    public void ClearObservations()
    {
        this.observations = new Observations();
    }
}
