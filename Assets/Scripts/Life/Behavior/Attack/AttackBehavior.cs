using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBehavior : AttackBehaviorDecisions, IDestroySensitive
{
    public static readonly List<Type> ATTACK_BEHAVIOR_TYPES = new List<Type>()
    {
        typeof(BombAttack),
        typeof(LaserAttack),
        typeof(OrbitAttack),
        typeof(SpikeAttack)
    };

    private Stopwatch cooldownSW;
    private Dictionary<DamageElement, DamageElement> trackedDamageElements =
        new Dictionary<DamageElement, DamageElement>();

    public virtual void Start()
    {
        this.cooldownSW = new Stopwatch();
    }

    // COOLDOWNS

    private float CB_GetScaledCooldown()
    {
        try
        {
            return this.DecideBaseCooldown() / Mathf.Sqrt(this.GetStats().AtkSpd);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return Mathf.Infinity;
    }

    // GET NEARBY ENEMIES

    /**
    Decide how to sort nearby enemies to attack
    */
    protected virtual List<GameObject> SortEnemiesInSight(Observations obs)
    {
        List<GameObject> nearbySorted = ObservationsUtils.SortEnemiesByDistance(
            this.gameObject,
            obs
        );

        return nearbySorted;
    }

    private List<GameObject> GetEnemiesInSight(Observations obs)
    {
        return this.DecidePickEnemiesInSight(this.SortEnemiesInSight(obs));
    }

    // TRACK DAMAGE ELEMENTS

    protected void TrackDmgEl(DamageElement dmgEl)
    {
        this.trackedDamageElements.Add(dmgEl, dmgEl);
    }

    /**
    Call to remove a DamageElement from tracked elements
    */
    protected void UntrackDmgEl(DamageElement dmgEl)
    {
        try
        {
            this.trackedDamageElements.Remove(dmgEl);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void ClearTrackedDamageElements()
    {
        foreach (DamageElement dmgEl in this.trackedDamageElements.Values)
        {
            dmgEl.ClearDmgCooldowns();
            this.UntrackDmgEl(dmgEl);
        }
    }

    private void ClearDmgCooldowns()
    {
        foreach (DamageElement dmgEl in this.trackedDamageElements.Values)
        {
            dmgEl.ClearDmgCooldowns();
        }
    }

    // INSTANTIATE DAMAGE ELEMENT

    protected void InstantiateDamageElements(List<GameObject> toAtk)
    {
        List<TargetInfo> targets = this.DecideTargets(toAtk);

        foreach (TargetInfo target in targets)
        {
            Vector3 pos = this.DecideInstantiatePosition(target);
            Quaternion rot = this.DecideInstantiateRotation(target);
            this.InstantiateDamageElement(target, pos, rot);
        }
    }

    private GameObject InstantiateDamageElement(TargetInfo target, Vector3 pos, Quaternion rot)
    {
        // 1. Instantiate GameObject
        GameObject atkGO = Instantiate(this.CB_GetDmgElGO(), pos, rot);

        // 2. Init DamageElement
        DamageElement dmgEl = atkGO.GetComponent<DamageElement>();
        dmgEl.Init(
            target,
            this.CB_MovementController,
            this.CB_DecideHandleIfReachTarget,
            this.CB_DecideReadyToDmg,
            this.CB_DecideEnemiesInHitbox,
            this.CB_GetShouldDmg,
            this.CB_GetScaledCooldown,
            this.CB_GetDmg,
            this.CB_DecideOnDmg,
            this.CB_OnDefeatEnemy,
            this.CB_HandleIfExhausted,
            this.CB_DecideAfterExecute,
            this.CB_BeforeDestroyGameObject,
            this.gameObject
        );

        // 3. Potentially track DamageElement
        if (this.DecideShouldTrackDamageEl(dmgEl))
            this.TrackDmgEl(dmgEl);

        return atkGO;
    }

    /**
    Get Attack GameObject
    */
    private GameObject CB_GetDmgElGO()
    {
        try
        {
            return Resources.Load(this.DecideDmgElResourcePath()) as GameObject;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return null;
    }

    public override void Execute(Observations obs)
    {
        // 0. Check cooldown
        if (!this.cooldownSW.HasElapsedStart(this.CB_GetScaledCooldown()))
            return;

        // 1. Get enemies to attack
        List<GameObject> toAtk = this.GetEnemiesInSight(obs);

        // == 0 enemies
        if (toAtk.Count == 0)
            this.ClearDmgCooldowns();
        // > 0 enemies
        else
            this.InstantiateDamageElements(toAtk);
    }

    protected virtual void CB_MovementController(DamageElement dmgEl, TargetInfo ti)
    {
        try
        {
            dmgEl.GetComponent<Rigidbody>().velocity =
                this.DecideMoveSpeed() * (ti.GetTarget() - dmgEl.transform.position);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    // READY TO DMG ----

    // GET SHOULD DMG ----

    /**
    Damage Element callback
    Return false if should not attack for any reason, else true
    */
    private bool CB_GetShouldDmg(GameObject go)
    {
        try
        {
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return false;
    }

    // GET DMG ----

    private float CB_GetDmg()
    {
        try
        {
            return this.DecideDmgMultiplier() * this.GetStats().Attack;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return 0;
    }

    // ON DEAD ----

    private void CB_OnDefeatEnemy(GameObject enemy) { }

    // HANDLE IF EXHAUSTED ----

    private bool CB_HandleIfExhausted(int hitCount, DamageElement dmgEl)
    {
        try
        {
            return this.CheckIfExhausted(hitCount, dmgEl);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return true;
    }

    /**
    Return true if Attack is exhausted, else false
    */
    protected virtual bool CheckIfExhausted(int hitCount, DamageElement dmgEl)
    {
        try
        {
            return hitCount >= this.DecideMaxHitCount();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return true;
    }

    // DESTROY ----

    private void CB_BeforeDestroyGameObject(DamageElement dmgEl)
    {
        try
        {
            this.UntrackDmgEl(dmgEl);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    /**
    Call from DestroyManager before destroying this GameObject
    Destroys all tracked DamageElements
    */
    public void OnDestroy()
    {
        List<DamageElement> elementsToDestroy = new List<DamageElement>();

        foreach (KeyValuePair<DamageElement, DamageElement> kvp in this.trackedDamageElements)
        {
            // Add elements to destroy to the separate list
            elementsToDestroy.Add(kvp.Value);
        }

        // Now, outside of the loop, you can safely destroy the GameObjects
        foreach (DamageElement dmgEl in elementsToDestroy)
        {
            dmgEl.DestroyGameObject();
        }

        // foreach (KeyValuePair<DamageElement, DamageElement> kvp in this.trackedDamageElements)
        // {
        //     // This will call CB_BeforeDestroy and then Destroy the DamageElement
        //     kvp.Value.Destroy();
        // }
    }

    // POSITIONING

    protected Quaternion FaceTarget(TargetInfo target)
    {
        return Quaternion.LookRotation(target.GetTarget() - this.transform.position);
    }
}
